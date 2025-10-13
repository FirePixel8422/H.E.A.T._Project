using UnityEngine;

public class EyeLookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float snappiness = 0.25f;



    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);


    private void OnUpdate()
    {
        if (player == null) return;

        Vector3 toPlayerDir = (player.transform.position - transform.position).normalized;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, toPlayerDir), snappiness * Time.deltaTime);
    }
}
