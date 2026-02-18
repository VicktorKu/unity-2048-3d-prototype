using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Cube Color Config", fileName = "CubeColorConfig")]
public class CubeColorConfig : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public int value;
        public Color color;
    }

    public Color defaultColor = Color.white;
    public List<Entry> entries = new List<Entry>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            int expected = 2 << i;

            if (e.value != expected)
            {
                e.value = expected;
                entries[i] = e;
            }
        }
    }
#endif

    public bool TryGetColor(int value, out Color color)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].value == value)
            {
                color = entries[i].color;
                return true;
            }
        }

        color = defaultColor;
        return false;
    }
}
