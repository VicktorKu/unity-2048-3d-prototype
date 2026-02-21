using UnityEngine;
using UnityEngine.UI;

public class LaunchPowerUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image fill;

    public void Awake()
    {
        HideFill();
    }

    public void SetPull01(float pull01)
    {
        if (root != null && !root.activeSelf) root.SetActive(true);
        if (fill != null) fill.fillAmount = pull01;
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