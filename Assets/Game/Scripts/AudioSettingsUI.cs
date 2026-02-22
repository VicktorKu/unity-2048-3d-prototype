using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private bool _isBinding;

    private void OnEnable()
    {
        TryBind();
    }

    private void TryBind()
    {
        var am = AudioManager.Instance;

        if (am == null)
        {
            Invoke(nameof(TryBind), 0f);
            return;
        }

        _isBinding = true;

        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(am.GetMusicVolume());

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(am.GetSfxVolume());

        _isBinding = false;
    }

    public void OnMusicSliderChanged()
    {
        if (_isBinding) return;

        var am = AudioManager.Instance;
        if (am == null) return;

        am.SetMusicVolume(musicSlider.value, save: true);
    }

    public void OnSfxSliderChanged()
    {
        if (_isBinding) return;

        var am = AudioManager.Instance;
        if (am == null) return;

        am.SetSfxVolume(sfxSlider.value, save: true);
    }
}