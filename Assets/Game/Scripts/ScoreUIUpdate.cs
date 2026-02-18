using TMPro;
using UnityEngine;

public class ScoreUIUpdate : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string prefix = "Score: ";

    private int _lastScore = int.MinValue;

    private void Awake()
    {
        if (scoreText == null)
            scoreText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        var sys = ScoreSystem.Instance;
        int current = sys != null ? sys.GetScore() : 0;

        if (current == _lastScore) return;

        _lastScore = current;
        scoreText.text = prefix + current;
    }
}