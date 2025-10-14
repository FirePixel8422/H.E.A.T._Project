using UnityEngine;

public class FlyingCameraMove : MonoBehaviour
{
    [Header("Waypoints Settings")]
    public Transform[] waypoints;       // Assign in the Inspector
    public float moveSpeed = 3f;        // How fast it moves
    public float arriveDistance = 0.2f; // How close counts as "arrived"

    private int currentIndex = 0;

    void Start()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned!");
            enabled = false;
        }
        else
        {
            // Start at the first waypoint
            transform.position = waypoints[0].position;
        }
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // Check if arrived at the current waypoint
        if (Vector3.Distance(transform.position, target.position) < arriveDistance)
        {
            currentIndex++;

            // Loop back to the start if at the end
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = 0;
            }
        }
    }
}
