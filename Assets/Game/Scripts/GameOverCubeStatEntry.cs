using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCubeStatEntry : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private TMP_Text countText;

    public void Setup(int value, int count, Color color)
    {
        if (bg) bg.color = color;
        if (valueText) valueText.text = value.ToString();
        if (countText) countText.text = $"{count}x";
    }
}