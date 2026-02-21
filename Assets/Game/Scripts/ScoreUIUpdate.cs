using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIUpdate : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestText;

    [SerializeField] private string scorePrefix = "Score: ";
    [SerializeField] private string bestPrefix = "Best: ";

    [Header("New best animation")]
    [SerializeField] private float punchScale = 1.12f;
    [SerializeField] private float punchDuration = 0.12f;

    private int _lastScore = int.MinValue;
    private int _lastBest = int.MinValue;

    private Coroutine _punchRoutine;

    private void Awake()
    {
        if (scoreText == null)
            scoreText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (ScoreSystem.Instance != null)
            ScoreSystem.Instance.OnScoreChanged += HandleScoreChanged;
    }

    private void Start()
    {
        UpdateUI(force: true, allowPunch: false);
    }

    private void OnDisable()
    {
        if (ScoreSystem.Instance != null)
            ScoreSystem.Instance.OnScoreChanged -= HandleScoreChanged;
    }

    private void HandleScoreChanged(int score, int best)
    {
        UpdateUI(force: false, allowPunch: true);
    }

    private void UpdateUI(bool force, bool allowPunch)
    {
        var sys = ScoreSystem.Instance;
        int score = sys != null ? sys.GetScore() : 0;
        int best = sys != null ? sys.GetBestScore() : 0;

        if (!force && score == _lastScore && best == _lastBest) return;

        bool bestIncreased = best > _lastBest;

        _lastScore = score;
        _lastBest = best;

        if (scoreText != null) scoreText.text = scorePrefix + score;
        if (bestText != null) bestText.text = bestPrefix + best;

        if (allowPunch && bestIncreased && bestText != null)
            Punch(bestText.transform);
    }

    private void Punch(Transform target)
    {
        if (_punchRoutine != null) StopCoroutine(_punchRoutine);
        _punchRoutine = StartCoroutine(PunchRoutine(target));
    }

    private IEnumerator PunchRoutine(Transform target)
    {
        Vector3 baseScale = Vector3.one;
        Vector3 upScale = baseScale * punchScale;
        float d = punchDuration;

        for (float t = 0; t < d; t += Time.unscaledDeltaTime)
        {
            target.localScale = Vector3.Lerp(baseScale, upScale, t / d);
            yield return null;
        }
        target.localScale = upScale;

        for (float t = 0; t < d; t += Time.unscaledDeltaTime)
        {
            target.localScale = Vector3.Lerp(upScale, baseScale, t / d);
            yield return null;
        }

        target.localScale = baseScale;
        _punchRoutine = null;
    }
}