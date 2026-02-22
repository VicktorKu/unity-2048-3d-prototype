using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string MusicKey = "audio_music_volume_v1";
    private const string SfxKey = "audio_sfx_volume_v1";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundLoop;
    [SerializeField] private AudioSource launchClip;
    [SerializeField] private AudioSource mergeClip;
    [SerializeField] private AudioSource gameOverClip;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 0.25f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadVolumes();
        ApplyVolumes();
    }

    private void Start()
    {
        PlayBackground();
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSfxVolume() => sfxVolume;

    public void SetMusicVolume(float v, bool save = true)
    {
        musicVolume = Mathf.Clamp01(v);
        ApplyVolumes();
        if (save) SaveVolumes();
    }

    public void SetSfxVolume(float v, bool save = true)
    {
        sfxVolume = Mathf.Clamp01(v);
        ApplyVolumes();
        if (save) SaveVolumes();
    }

    public void ApplyVolumes()
    {
        if (backgroundLoop != null)
            backgroundLoop.volume = musicVolume;

        if (launchClip != null)
            launchClip.volume = sfxVolume;

        if (mergeClip != null)
            mergeClip.volume = sfxVolume;

        if (gameOverClip != null)
            gameOverClip.volume = sfxVolume;
    }

    private void LoadVolumes()
    {
        if (PlayerPrefs.HasKey(MusicKey))
            musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicKey));

        if (PlayerPrefs.HasKey(SfxKey))
            sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxKey));
    }

    private void SaveVolumes()
    {
        PlayerPrefs.SetFloat(MusicKey, musicVolume);
        PlayerPrefs.SetFloat(SfxKey, sfxVolume);
        PlayerPrefs.Save();
    }

    public void PlayBackground()
    {
        if (backgroundLoop != null && !backgroundLoop.isPlaying)
            backgroundLoop.Play();
    }

    public void PlayLaunch()
    {
        if (launchClip != null)
            launchClip.Play();
    }

    public void PlayMerge()
    {
        if (mergeClip != null)
            mergeClip.Play();
    }

    public void PlayGameOver()
    {
        if (gameOverClip != null)
            gameOverClip.Play();
    }
}