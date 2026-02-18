using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CubeMergeDetector : MonoBehaviour
{
    [Header("Proximity merge")]
    [Min(0f)] public float extraRadius = 0.1f;
    
    [Header("Merge impulse")]
    [Min(0f)] public float mergeUpImpulse = 2f;
    [Min(0f)] public float mergeSideImpulse = 0.5f;
    
    public LayerMask cubeLayer = ~0;  

    private CubeEntity _self;
    private Rigidbody _rb;
    private BoxCollider _col;
    private bool _mergeLocked;

    private void Awake()
    {
        _self = GetComponent<CubeEntity>();
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        if (_mergeLocked) return;
        if (_self == null) return;

        TryProximityMerge();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_mergeLocked) return;

        var otherEntity = collision.collider.GetComponent<CubeEntity>();
        var otherMerge = collision.collider.GetComponent<CubeMergeDetector>();
        if (otherEntity == null || otherMerge == null) return;

        if (otherEntity.Value != _self.Value) return;

        if (gameObject.GetInstanceID() > otherEntity.gameObject.GetInstanceID()) return;

        LockBoth(otherMerge);
        PerformMerge(otherEntity);
    }

    private void TryProximityMerge()
    {
        float half = GetHalfSizeWorld();
        float radius = half + extraRadius;

        var hits = Physics.OverlapSphere(transform.position, radius, cubeLayer, QueryTriggerInteraction.Ignore);

        CubeEntity best = null;
        CubeMergeDetector bestMerge = null;
        float bestDistSq = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].attachedRigidbody == _rb) continue;

            var otherEntity = hits[i].GetComponent<CubeEntity>();
            var otherMerge = hits[i].GetComponent<CubeMergeDetector>();
            var otherRb = hits[i].attachedRigidbody;

            if (otherEntity == null || otherMerge == null || otherRb == null) continue;
            if (otherMerge._mergeLocked) continue;
            if (otherEntity.Value != _self.Value) continue;

            if (gameObject.GetInstanceID() > otherEntity.gameObject.GetInstanceID()) continue;

            float dSq = (otherEntity.transform.position - transform.position).sqrMagnitude;
            if (dSq < bestDistSq)
            {
                bestDistSq = dSq;
                best = otherEntity;
                bestMerge = otherMerge;
            }
        }

        if (best == null) return;

        LockBoth(bestMerge);

        best.transform.position = transform.position;

        PerformMerge(best);
    }

    private void LockBoth(CubeMergeDetector other)
    {
        _mergeLocked = true;
        other._mergeLocked = true;
    }

    private void PerformMerge(CubeEntity other)
    {
        _self.DoubleValue();

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomSide = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized;

            Vector3 impulse =
                Vector3.up * mergeUpImpulse +
                randomSide * mergeSideImpulse;

            rb.AddForce(impulse, ForceMode.Impulse);
        }

        Destroy(other.gameObject);
        _mergeLocked = false;
    }

    private float GetHalfSizeWorld()
    {
        float halfX = _col.size.x * 0.5f * transform.lossyScale.x;
        float halfY = _col.size.y * 0.5f * transform.lossyScale.y;
        float halfZ = _col.size.z * 0.5f * transform.lossyScale.z;
        return Mathf.Max(halfX, halfY, halfZ);
    }
}
