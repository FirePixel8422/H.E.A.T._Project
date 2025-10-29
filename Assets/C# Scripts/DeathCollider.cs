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
        if (other.transform.TryGetComponent(out SmartHitBox hitBox))
        {
            hitBox.DealDamageToTargetObject(int.MaxValue, false, default, default, out _);
        }
    }
}
