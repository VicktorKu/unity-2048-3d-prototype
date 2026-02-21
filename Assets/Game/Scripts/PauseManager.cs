using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    private bool _isPaused;

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TogglePause()
    {
        if (_isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}