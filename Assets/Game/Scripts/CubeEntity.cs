using System;
using UnityEngine;

public class CubeEntity : MonoBehaviour
{
    [SerializeField] private int value = 2;
    
    public int Value => value;
    
    public event Action<int> OnValueChanged;

    private void Awake()
    {
        NotifyValueChanged();
    }

    public void SetValue(int newValue)
    {
        if (!IsPowerOfTwo(newValue))
        {
            Debug.LogError("Value must be power of two.");
            return;
        }

        value = newValue;
        NotifyValueChanged();
    }

    public void DoubleValue()
    {
        OnValueChanged?.Invoke(value);
        value *= 2;
        NotifyValueChanged();
    }

    private void NotifyValueChanged()
    {
        var labels = GetComponent<CubeFaceLabels>();
        if (labels != null)
            labels.SetValue(value);

        var color = GetComponent<CubeColorVisual>();
        if (color != null)
            color.SetValue(value);
    }

    private bool IsPowerOfTwo(int number)
    {
        return number > 0 && (number & (number - 1)) == 0;
    }
}
