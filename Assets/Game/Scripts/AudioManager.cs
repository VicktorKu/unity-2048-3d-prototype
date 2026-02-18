using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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
    }

    private void Start()
    {
        ApplyVolumes();
        PlayBackground();
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