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

    [Header("Merge condition")]
    [Min(0f)] public float minTowardSpeed = 0.05f;

    [Header("Velocity limits")]
    [Min(0f)] public float maxLinearSpeed = 8f;
    [Min(0f)] public float maxAngularSpeed = 20f;

    [Header("Merge cooldown")]
    [Min(0f)] public float mergeCooldown = 0.05f;

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

        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        if (_self == null) return;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_mergeLocked) return;

        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        var otherEntity = collision.collider.GetComponent<CubeEntity>();
        var otherMerge = collision.collider.GetComponent<CubeMergeDetector>();
        if (otherEntity == null || otherMerge == null) return;

        if (otherEntity.Value != _self.Value) return;
        if (gameObject.GetInstanceID() > otherEntity.gameObject.GetInstanceID()) return;

        var otherRb = collision.rigidbody;
        if (_rb != null && _rb.isKinematic) return;
        if (otherRb.isKinematic) return;

        Vector3 dirToOther = (transform.position - otherEntity.transform.position).normalized;
        float towardSpeed = Vector3.Dot(collision.relativeVelocity, dirToOther);

        if (towardSpeed < minTowardSpeed) return;


        LockBoth(otherMerge);
        PerformMerge(otherEntity);
    }

    private void LockBoth(CubeMergeDetector other)
    {
        _mergeLocked = true;
        other._mergeLocked = true;
    }

    private void PerformMerge(CubeEntity other)
    {
        _self.DoubleValue();
        _self.PlayMergeEffect();

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
            ClampVelocity(rb);
        }
        UnlockAfterDelay();

        var otherState = other.GetComponent<StartZoneState>();
        if (otherState != null)
        {
            var zone = FindObjectOfType<StartZoneGameOver>();
            if (zone != null)
                zone.ForceCleanupState(otherState);
        }
        Destroy(other.gameObject);

        AudioManager.Instance?.PlayMerge();
    }

    private float GetHalfSizeWorld()
    {
        float halfX = _col.size.x * 0.5f * transform.lossyScale.x;
        float halfY = _col.size.y * 0.5f * transform.lossyScale.y;
        float halfZ = _col.size.z * 0.5f * transform.lossyScale.z;
        return Mathf.Max(halfX, halfY, halfZ);
    }

    private void ClampVelocity(Rigidbody rb)
    {
        if (rb.velocity.sqrMagnitude > maxLinearSpeed * maxLinearSpeed)
            rb.velocity = rb.velocity.normalized * maxLinearSpeed;

        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, maxAngularSpeed);
    }
    private void UnlockAfterDelay()
    {
        StartCoroutine(UnlockRoutine());
    }

    private System.Collections.IEnumerator UnlockRoutine()
    {
        yield return new WaitForSeconds(mergeCooldown);
        _mergeLocked = false;
    }
}
