using UnityEngine;
using TMPro;

[ExecuteAlways]
public class CubeFaceLabels : MonoBehaviour
{
    [Header("Value")]
    [Min(2)] public int value = 2;

    [Header("References")]
    [SerializeField] private Transform textsRoot;
    [SerializeField] private TextMeshPro textTemplate;

    [Header("Layout")]
    [Min(0.0001f)] public float surfaceOffset = 0.002f;
    [Range(0.1f, 1f)] public float textScaleFactor = 0.35f;

    private TextMeshPro[] _texts;

    private static readonly Vector3[] Directions =
    {
        Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down
    };

    void OnValidate()
    {
        if (!textsRoot || !textTemplate) return;
        EnsureTexts();
        UpdateTexts();
        Reposition();
    }

    void Awake()
    {
        if (!textsRoot || !textTemplate) return;
        EnsureTexts();
        UpdateTexts();
        Reposition();
    }

    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateTexts();
    }

    private void EnsureTexts()
    {
        if (_texts != null && _texts.Length == 6 && _texts[0] != null) return;

        _texts = new TextMeshPro[6];

        var existing = textsRoot.GetComponentsInChildren<TextMeshPro>(true);
        if (existing.Length >= 6)
        {
            for (int i = 0; i < 6; i++) _texts[i] = existing[i];
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            var t = Instantiate(textTemplate, textsRoot);
            t.name = $"FaceText_{i}";
            t.gameObject.SetActive(true);
            _texts[i] = t;
        }

        if (textTemplate.transform.parent == textsRoot)
            textTemplate.gameObject.SetActive(false);
    }

    private void UpdateTexts()
    {
        if (_texts == null) return;
        string s = value.ToString();
        for (int i = 0; i < 6; i++)
            if (_texts[i]) _texts[i].text = s;
    }

    private void Reposition()
    {
        if (_texts == null) return;

        var col = GetComponent<BoxCollider>();
        Vector3 colSize = col ? col.size : Vector3.one;

        Vector3 half = Vector3.Scale(colSize, transform.localScale) * 0.5f;

        for (int i = 0; i < 6; i++)
        {
            Vector3 dir = Directions[i];

            Vector3 localPos = new Vector3(
                dir.x * half.x,
                dir.y * half.y,
                dir.z * half.z
            );


            float avgScale = (Mathf.Abs(transform.localScale.x) + Mathf.Abs(transform.localScale.y) + Mathf.Abs(transform.localScale.z)) / 3f;
            float localOffset = avgScale > 0f ? surfaceOffset / avgScale : surfaceOffset;

            localPos += dir * localOffset;

            _texts[i].transform.localPosition = localPos;

            Vector3 up = GetUpVector(dir);
            _texts[i].transform.localRotation = Quaternion.LookRotation(dir, up);

            float faceMin = GetFaceMinSize(half, dir) * 2f;
            float s = faceMin * textScaleFactor;
            _texts[i].transform.localScale = Vector3.one * s;
        }
    }

    private float GetFaceMinSize(Vector3 half, Vector3 dir)
    {
        if (dir == Vector3.up || dir == Vector3.down)
            return Mathf.Min(half.x, half.z);

        if (dir == Vector3.right || dir == Vector3.left)
            return Mathf.Min(half.y, half.z);

        return Mathf.Min(half.x, half.y);
    }

    private static Vector3 GetUpVector(Vector3 dir)
    {
        if (dir == Vector3.up) return Vector3.forward;
        if (dir == Vector3.down) return Vector3.back;

        return Vector3.up;
    }
}