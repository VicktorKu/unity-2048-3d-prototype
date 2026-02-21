using UnityEngine;
using UnityEngine.UI;

public class DeathZoneUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image fill;

    private void Awake()
    {
        HideFill();
    }

    public void SetProgress01(float t01)
    {
        if (root != null && !root.activeSelf) root.SetActive(true);
        if (fill != null) fill.fillAmount = Mathf.Clamp01(t01);
    }

    public void HideFill()
    {
        if (fill != null) fill.fillAmount = 0f;
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }
}