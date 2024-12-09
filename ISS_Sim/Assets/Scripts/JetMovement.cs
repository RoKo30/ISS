using UnityEngine;

public class JetMovement : MonoBehaviour
{
    [SerializeField]
    private float forwardSpeed = 50f; // Forward speed of the jet

    [SerializeField]
    private float turningSpeed = 1f; // Speed of the jet's turning

    [SerializeField]
    private float maxTurnAngle = 45f; // Maximum angle for random turning

    [SerializeField]
    private float raycastDistance = 200f; // Distance of the raycast for obstacle detection

    private Quaternion targetRotation; // Target rotation for the jet

    void Start()
    {
        // Initialize with the current rotation
        targetRotation = transform.rotation;

        // Start changing direction periodically
        InvokeRepeating(nameof(ChangeDirection), 0, 7f); // Adjust time as needed
    }

    void Update()
    {
        // Perform raycast to detect obstacles
        CheckForObstacles();

        // Move forward at a constant speed
        transform.Translate(Vector3.forward * (forwardSpeed * Time.deltaTime));

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
    }

    private void ChangeDirection()
    {
        // Randomly pick a new rotation within the max turn angles
        Vector3 randomDirection = new Vector3(
            Random.Range(-maxTurnAngle, maxTurnAngle), // Random tilt left/right
            Random.Range(-maxTurnAngle * 0.5f, maxTurnAngle * 0.5f), // Random tilt up/down
            0 // Keep forward as forward
        );

        // Calculate the new target rotation
        targetRotation = Quaternion.Euler(randomDirection) * transform.rotation;
    }

    private void CheckForObstacles()
    {
        // Cast a ray forward from the jet's position
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Check if the ray hits anything within the specified distance
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            //Debug.Log($"Obstacle detected: {hit.collider.name}");

            // Turn upwards to avoid the obstacle
            Vector3 upwardDirection = new Vector3(-maxTurnAngle, 0, 0); // Turn upwards
            targetRotation = Quaternion.Euler(upwardDirection) * transform.rotation;
        }

        // Optional: Visualize the ray in the Scene view (for debugging)
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red);
    }
}
