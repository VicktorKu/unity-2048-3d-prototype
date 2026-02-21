using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestText;

    private void Awake()
    {
        Instance = this;
        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        if (panel != null) panel.SetActive(visible);
        else gameObject.SetActive(visible);
    }

    public void SetData(int score, int best)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (bestText != null) bestText.text = $"Best: {best}";
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}