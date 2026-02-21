using System;
using System.Collections;
using UnityEngine;

public class CubeEntity : MonoBehaviour
{
    [SerializeField] private int value = 2;
    public bool IsLaunched { get; private set; }
    public float LaunchedAt { get; private set; }

    public int Value => value;
    
    public event Action<int> OnValueChanged;

    private Vector3 _baseScale;
    private Coroutine _mergeScaleRoutine;

    private CubeFaceLabels _labels;
    private CubeColorVisual _color;

    private void Awake()
    {
        _labels = GetComponent<CubeFaceLabels>();
        _color = GetComponent<CubeColorVisual>();

        _baseScale = transform.localScale;
        NotifyValueChanged();
    }

    public void SetValue(int newValue)
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        if (!IsPowerOfTwo(newValue))
        {
            return;
        }

        value = newValue;
        NotifyValueChanged();
    }

    public void DoubleValue()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        OnValueChanged?.Invoke(value);
        value *= 2;
        NotifyValueChanged();
    }

    private void NotifyValueChanged()
    {
        _labels?.SetValue(value);
        _color?.SetValue(value);
    }

    private bool IsPowerOfTwo(int number)
    {
        return number > 0 && (number & (number - 1)) == 0;
    }
    public void PlayMergeEffect()
    {
        if (_mergeScaleRoutine != null)
            StopCoroutine(_mergeScaleRoutine);
        
        transform.localScale = _baseScale;
        _mergeScaleRoutine = StartCoroutine(MergeScaleRoutine(_baseScale));
    }

    public void MarkLaunched()
    {
        IsLaunched = true;
        LaunchedAt = Time.time;
    }

    public void CacheBaseScale()
    {
        _baseScale = transform.localScale;
    }

    private IEnumerator MergeScaleRoutine(Vector3 baseScale)
    {
        Vector3 target = baseScale * 1.15f;

        float duration = 0.08f;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float lerp = t / duration;
            transform.localScale = Vector3.Lerp(baseScale, target, lerp);
            yield return null;
        }
        transform.localScale = target;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float lerp = t / duration;
            transform.localScale = Vector3.Lerp(target, baseScale, lerp);
            yield return null;
        }

        transform.localScale = baseScale;
        _mergeScaleRoutine = null;
    }
}
