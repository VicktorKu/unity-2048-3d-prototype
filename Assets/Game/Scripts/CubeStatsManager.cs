using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeStatsManager : MonoBehaviour
{
    public static CubeStatsManager Instance { get; private set; }

    private const string PrefKey = "cube_stats_lifetime_v1";

    private readonly Dictionary<int, int> _roundMerged = new();
    public IReadOnlyDictionary<int, int> RoundMerged => _roundMerged;

    private readonly Dictionary<int, int> _lifetimeMerged = new();
    public IReadOnlyDictionary<int, int> LifetimeMerged => _lifetimeMerged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        LoadLifetime();
    }

    public void StartNewRound()
    {
        _roundMerged.Clear();
    }

    public void RegisterMergedCube(int value)
    {
        if (_roundMerged.TryGetValue(value, out var r)) _roundMerged[value] = r + 1;
        else _roundMerged[value] = 1;

        if (_lifetimeMerged.TryGetValue(value, out var l)) _lifetimeMerged[value] = l + 1;
        else _lifetimeMerged[value] = 1;
    }

    public void RegisterCubeCreated(int value)
    {
        if (_roundMerged.TryGetValue(value, out var r)) _roundMerged[value] = r + 1;
        else _roundMerged[value] = 1;

        if (_lifetimeMerged.TryGetValue(value, out var l)) _lifetimeMerged[value] = l + 1;
        else _lifetimeMerged[value] = 1;
    }

    public void CommitLifetime()
    {
        SaveLifetime();
    }

    [System.Serializable]
    private class Entry { public int value; public int count; }

    [System.Serializable]
    private class Data { public List<Entry> entries = new(); }

    private void LoadLifetime()
    {
        _lifetimeMerged.Clear();
        if (!PlayerPrefs.HasKey(PrefKey)) return;

        var json = PlayerPrefs.GetString(PrefKey, "");
        if (string.IsNullOrEmpty(json)) return;

        var data = JsonUtility.FromJson<Data>(json);
        if (data?.entries == null) return;

        foreach (var e in data.entries)
            _lifetimeMerged[e.value] = Mathf.Max(0, e.count);
    }

    private void SaveLifetime()
    {
        var data = new Data
        {
            entries = _lifetimeMerged
                .Select(kv => new Entry { value = kv.Key, count = kv.Value })
                .ToList()
        };

        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(PrefKey, json);
        PlayerPrefs.Save();
    }
}