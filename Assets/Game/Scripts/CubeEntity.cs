using UnityEngine;
using TMPro;

public class CubeEntity : MonoBehaviour
{
    [SerializeField] private int value = 2;
    [SerializeField] private TextMeshPro valueText;

    public int Value => value;

    private void Awake()
    {
        UpdateVisual();
    }

    public void SetValue(int newValue)
    {
        if (!IsPowerOfTwo(newValue))
        {
            Debug.LogError("Value must be power of two.");
            return;
        }

        value = newValue;
        UpdateVisual();
    }

    public void DoubleValue()
    {
        value *= 2;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (valueText != null)
            valueText.text = value.ToString();
    }

    private bool IsPowerOfTwo(int number)
    {
        return number > 0 && (number & (number - 1)) == 0;
    }
}
