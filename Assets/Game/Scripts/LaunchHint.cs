using UnityEngine;

public class LaunchHint : MonoBehaviour
{
    [SerializeField] private GameObject hintRoot;
    [SerializeField] private bool forceShowInEditor = true;
    private bool _wasHidden;

    private const string LaunchHintKey = "launch_hint_shown";

    private void Awake()
    {
#if UNITY_EDITOR
        if (hintRoot != null && forceShowInEditor)
            hintRoot.SetActive(true);
#else
        if (PlayerPrefs.GetInt(LaunchHintKey, 0) == 1)
            hintRoot?.SetActive(false);
#endif
    }

    public void OnFirstLaunch()
    {
        if (_wasHidden) return;

        _wasHidden = true;

        if (hintRoot != null)
            hintRoot.SetActive(false);

#if !UNITY_EDITOR
        PlayerPrefs.SetInt(LaunchHintKey, 1);
        PlayerPrefs.Save();
#endif
    }
}