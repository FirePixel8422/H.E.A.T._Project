using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DeathCollider : MonoBehaviour
{

    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnCollisionEnter(Collision other)
    {
        DebugLogger.LogWarning("touched");
        if (other.transform.TryGetComponent(out SmartHitBox hitBox))
        {
        DebugLogger.LogWarning("player touched");
            hitBox.DealDamageToTargetObject(int.MaxValue, false, default, default, out _);
        }
    }
}
