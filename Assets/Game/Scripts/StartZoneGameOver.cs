using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZoneGameOver : MonoBehaviour
{
    [Header("How long cube may stay inside start zone before it is considered a loss")]
    [SerializeField] private float insideTimeout = 0.45f;

    [Header("Cube is considered stopped if speed is below this threshold")]
    [SerializeField] private float stoppedSpeed = 0.05f;

    [Header("Line visuals")]
    [SerializeField] private Renderer lineRenderer;
    [SerializeField] private Color safeColor = new Color(1f, 0.9f, 0.2f, 1f);
    [SerializeField] private Color dangerColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private string colorProperty = "_BaseColor";
    private int _preferredColorId;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private bool _gameOver;

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
        if (_gameOver) return;

        var cube = other.GetComponent<CubeEntity>();
        if (cube == null) return;
        if (!cube.IsLaunched) return;

        var state = other.GetComponent<StartZoneState>();
        if (state == null) state = other.gameObject.AddComponent<StartZoneState>();

        state.IsInside = true;
        state.LastEnterTime = Time.time;

        BeginDanger(state);

        if (_checks.TryGetValue(state, out var running) && running != null)
            StopCoroutine(running);

        _checks[state] = StartCoroutine(CheckStayedInside(other, state, state.LastEnterTime));
    }

    private void OnTriggerExit(Collider other)
    {
        var cube = other.GetComponent<CubeEntity>();
        if (cube == null) return;
        if (!cube.IsLaunched) return;

        var state = other.GetComponent<StartZoneState>();
        if (state == null) return;

        state.IsInside = false;

        if (_checks.TryGetValue(state, out var running) && running != null)
            StopCoroutine(running);

        _checks.Remove(state);

        EndDanger(state);
    }

    private IEnumerator CheckStayedInside(Collider cubeCollider, StartZoneState state, float enterTimeSnapshot)
    {
        yield return new WaitForSeconds(insideTimeout);

        if (_gameOver) yield break;

        if (!state.IsInside) yield break;
        if (!Mathf.Approximately(state.LastEnterTime, enterTimeSnapshot)) yield break;

        var cube = cubeCollider.GetComponent<CubeEntity>();
        if (cube == null) yield break;
        if (!cube.IsLaunched) yield break;

        var rb = cubeCollider.attachedRigidbody;

        if (rb != null)
        {
            if (rb.IsSleeping() || rb.velocity.sqrMagnitude <= stoppedSpeed * stoppedSpeed)
            {
                _gameOver = true;
                ApplyLineColor(dangerColor);

                GameOverController.Instance?.TriggerGameOver();
            }
            else
            {
                _checks[state] = StartCoroutine(CheckStayedInside(cubeCollider, state, state.LastEnterTime));
            }
        }
        else
        {
            _gameOver = true;
            ApplyLineColor(dangerColor);
            GameOverController.Instance?.TriggerGameOver();
        }
    }

    private void BeginDanger(StartZoneState state)
    {
        if (_dangerStates.Add(state))
            ApplyLineColor(dangerColor);
    }

    private void EndDanger(StartZoneState state)
    {
        if (_dangerStates.Remove(state) && _dangerStates.Count == 0 && !_gameOver)
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