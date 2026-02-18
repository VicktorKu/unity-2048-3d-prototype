using UnityEngine;

public class StartZoneGameOver : MonoBehaviour
{
    [SerializeField] private float confirmExitDelay = 0.15f;

    private bool _gameOver;

    private void OnTriggerEnter(Collider other)
    {
        if (_gameOver) return;

        var cube = other.GetComponent<CubeEntity>();
        if (cube == null) return;

        var state = other.GetComponent<StartZoneState>();
        if (state == null) state = other.gameObject.AddComponent<StartZoneState>();

        state.IsInside = true;

        if (!state.HasLeftStartZone) return;

        _gameOver = true;
        Debug.Log("GAME OVER: cube re-entered start zone");
        Time.timeScale = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        var cube = other.GetComponent<CubeEntity>();
        if (cube == null) return;

        var state = other.GetComponent<StartZoneState>();
        if (state == null) state = other.gameObject.AddComponent<StartZoneState>();

        state.IsInside = false;
        state.ExitTime = Time.time;

        StartCoroutine(ConfirmExit(state));
    }

    private System.Collections.IEnumerator ConfirmExit(StartZoneState state)
    {
        float t = state.ExitTime;
        yield return new WaitForSeconds(confirmExitDelay);

        if (!state.IsInside && Mathf.Approximately(state.ExitTime, t))
            state.HasLeftStartZone = true;
    }
}