using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance { get; private set; }

    [SerializeField] private GameObject root;

    [Header("Optional Restart Key")]
    [SerializeField] private KeyCode restartKey = KeyCode.R;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetVisible(false);
    }

    private void Update()
    {
        if (GameStateManager.Instance == null) return;
        if (!GameStateManager.Instance.IsPaused()) return;

        if (Input.GetKeyDown(restartKey))
        {
            RestartLevel();
        }
    }

    public void SetVisible(bool visible)
    {
        if (root != null)
            root.SetActive(visible);
        else
            gameObject.SetActive(visible);
    }

    public void OnResumeClicked()
    {
        GameStateManager.Instance?.ChangeState(GameStateId.Playing);
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}