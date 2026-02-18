using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CubeColorVisual : MonoBehaviour
{
    [SerializeField] private CubeColorConfig config;
    [SerializeField] private Renderer targetRenderer;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
    }

    public void SetValue(int value)
    {
        if (targetRenderer == null || config == null) return;

        if (_mpb == null) _mpb = new MaterialPropertyBlock();

        if (!config.TryGetColor(value, out var c))
            c = config.defaultColor;

        targetRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(BaseColorId, c);
        _mpb.SetColor(ColorId, c);
        targetRenderer.SetPropertyBlock(_mpb);
    }
}