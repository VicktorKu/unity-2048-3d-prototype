using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZoneGameOver : MonoBehaviour
{
    [Header("How long cube may stay inside start zone before it is considered a loss")]
    [SerializeField] private float insideTimeout = 2f;

    [Header("Line visuals")]
    [SerializeField] private Renderer lineRenderer;
    [SerializeField] private Color safeColor = new Color(1f, 0.9f, 0.2f, 1f);
    [SerializeField] private Color dangerColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private string colorProperty = "_BaseColor";

    [SerializeField] private DeathZoneUI deathUI;

    private int _preferredColorId;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private readonly Dictionary<StartZoneState, Coroutine> _checks = new();
    private readonly HashSet<StartZoneState> _dangerStates = new();
    
    private MaterialPropertyBlock _mpb;


    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        _preferredColorId = Shader.PropertyToID(colorProperty);
        ApplyLineColor(safeColor);
    }

    private void OnDisable()
    {
        _dangerStates.Clear();
        ApplyLineColor(safeColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        var cube = other.GetComponent<CubeEntity>();
        if (cube == null) return;
        if (!cube.IsLaunched) return;

        var state = other.GetComponent<StartZoneState>();
        if (state == null) state = other.gameObject.AddComponent<StartZoneState>();

        state.IsInside = true;
        state.LastEnterTime = Time.time;

        BeginDanger(state);
        deathUI?.HideFill();


        if (_checks.TryGetValue(state, out var running) && running != null)
            StopCoroutine(running);

        _checks[state] = StartCoroutine(CheckStayedInside(other, state, state.LastEnterTime));
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        var cube = other.GetComponent<CubeEntity>();
        if (cube == null) return;
        if (!cube.IsLaunched) return;

        var state = other.GetComponent<StartZoneState>();
        if (state == null) return;

        state.IsInside = false;

        if (_checks.TryGetValue(state, out var running) && running != null)
            StopCoroutine(running);

        _checks.Remove(state);

        deathUI?.HideFill();

        EndDanger(state);
    }

    private IEnumerator CheckStayedInside(Collider cubeCollider, StartZoneState state, float enterTimeSnapshot)
    {
        float startTime = Time.time;

        while (true)
        {
            if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
                yield break;

            if (cubeCollider == null || cubeCollider.gameObject == null)
            {
                _checks.Remove(state);
                deathUI?.HideFill();
                deathUI?.Hide();
                yield break;
            }

            if (state == null) yield break;
            if (!state.IsInside) { deathUI?.HideFill(); deathUI?.Hide(); yield break; }
            if (!Mathf.Approximately(state.LastEnterTime, enterTimeSnapshot)) yield break;

            float elapsed = Time.time - startTime;
            float t01 = elapsed / insideTimeout;

            deathUI?.SetProgress01(t01);

            if (t01 >= 1f)
                break;

            yield return null;
        }

        var cube = cubeCollider.GetComponent<CubeEntity>();
        if (cube == null) { _checks.Remove(state); yield break; }
        if (!cube.IsLaunched) yield break;

        var rb = cubeCollider.attachedRigidbody;
        if (rb != null && !rb.IsSleeping())
        {
            state.LastEnterTime = Time.time;
            _checks[state] = StartCoroutine(CheckStayedInside(cubeCollider, state, state.LastEnterTime));
            yield break;
        }

        ApplyLineColor(dangerColor);
        GameOverService.Instance?.TriggerGameOver();
    }

    private void BeginDanger(StartZoneState state)
    {
        if (_dangerStates.Add(state))
            ApplyLineColor(dangerColor);
    }

    private void EndDanger(StartZoneState state)
    {
        if (_dangerStates.Remove(state) && _dangerStates.Count == 0)
            ApplyLineColor(safeColor);
    }

    private void ApplyLineColor(Color c)
    {
        if (lineRenderer == null) return;
        if (_mpb == null) _mpb = new MaterialPropertyBlock();

        var mat = lineRenderer.sharedMaterial;
        if (mat == null) return;

        int idToUse;

        if (!string.IsNullOrEmpty(colorProperty) && mat.HasProperty(_preferredColorId))
            idToUse = _preferredColorId;
        else if (mat.HasProperty(BaseColorId))
            idToUse = BaseColorId;
        else if (mat.HasProperty(ColorId))
            idToUse = ColorId;
        else
            return;

        lineRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(idToUse, c);
        lineRenderer.SetPropertyBlock(_mpb);
    }
}