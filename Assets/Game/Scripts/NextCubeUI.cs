using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextCubeUI : MonoBehaviour
{
    [SerializeField] private CubeSpawner spawner;

    [Header("Text")]
    [SerializeField] private TMP_Text nextText;

    [Header("Preview")]
    [SerializeField] private Image previewImage;
    [SerializeField] private CubeColorConfig colorConfig;

    private void OnEnable()
    {
        if (spawner != null)
            spawner.OnNextValueChanged += HandleNext;
    }

    private void OnDisable()
    {
        if (spawner != null)
            spawner.OnNextValueChanged -= HandleNext;
    }

    private void Start()
    {
        if (spawner != null)
            HandleNext(spawner.NextValue);
    }

    private void HandleNext(int value)
    {
        if (nextText != null)
            nextText.text = value.ToString();

        if (previewImage != null && colorConfig != null)
        {
            if (colorConfig.TryGetColor(value, out var c))
                previewImage.color = c;
            else
                previewImage.color = colorConfig.defaultColor;
        }
    }
}