using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZoneGameOver : MonoBehaviour
{
    [Header("How long cube may stay inside start zone before it is considered a loss")]
    [SerializeField] private float insideTimeout = 0.45f;

    [Header("Cube is considered stopped if speed is below this threshold")]
    [SerializeField] private float stoppedSpeed = 0.05f;

    private bool _gameOver;

    private readonly Dictionary<StartZoneState, Coroutine> _checks = new();

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
    }

    private IEnumerator CheckStayedInside(Collider cubeCollider, StartZoneState state, float enterTimeSnapshot)
    {
        yield return new WaitForSeconds(insideTimeout);

        if (_gameOver) yield break;

        if (!state.IsInside) yield break;
        if (!Mathf.Approximately(state.LastEnterTime, enterTimeSnapshot)) yield break;

        var cube = cubeCollider.GetComponent<CubeEntity>();
        if (cube == null) yield break;

        if (!cube.IsLaunched)
        {
            yield break;
        }

        var rb = cubeCollider.attachedRigidbody;
        if (rb != null)
        {
            if (rb.IsSleeping() || rb.velocity.sqrMagnitude <= stoppedSpeed * stoppedSpeed)
            {
                _gameOver = true;
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
            GameOverController.Instance?.TriggerGameOver();
        }
    }
}