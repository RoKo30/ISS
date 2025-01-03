using UnityEngine;

public class MissileMovement : MonoBehaviour
{
    [SerializeField]
    private float forwardSpeed = 50f; // Forward speed of the missile

    [SerializeField]
    private float turningSpeed = 2f; // Speed of the missile's rotation adjustment

    [SerializeField]
    private float raycastDistance = 100f; // Distance of raycasts for detection

    [SerializeField]
    private int rayCount = 30; // Number of rays in the cone

    [SerializeField]
    private float coneAngle = 30f; // Angle of the detection cone

    private GameObject target; // The jet the missile is chasing
    
    [SerializeField]
    private GameObject explosionPrefab; // Prefab for the explosion effect

    [SerializeField]
    private GameObject collisionCameraPrefab; // Prefab for the camera object

    [SerializeField]
    private Vector3 cameraOffset = new Vector3(0, 5, -10); // Offset for the camera

    private void Start()
    {
        // Find the target (for example, by tag)
        GameObject jet = GameObject.FindWithTag("Jet");
        if (jet != null)
        {
            target = jet;
        }
        else
        {
            Debug.LogWarning("No jet found in the scene with the 'Jet' tag.");
        }
    }

    private void Update()
    {
        // Apply constant forward movement
        transform.Translate(Vector3.forward * (forwardSpeed * Time.deltaTime));

        // If there's a target, check for collision and adjust rotation
        if (target != null)
        {
            bool targetDetected = Perform3DConeRaycast();

            if (targetDetected)
            {
                RotateTowardsTarget();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the missile collided with the jet
        if (collision.transform.CompareTag("Jet"))
        {
            Debug.Log("Missile hit the jet!");

            // Create the explosion at the collision point
            InstantiateExplosion(collision.contacts[0].point);

            // Create and position the camera above the collision point
            InstantiateCamera(collision.contacts[0].point);

            // Destroy the jet
            Destroy(collision.gameObject);

            // Destroy the missile
            Destroy(gameObject);
        }
    }
    private void InstantiateExplosion(Vector3 position)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Explosion prefab is not assigned.");
        }
    }

    private void InstantiateCamera(Vector3 collisionPoint)
    {
        if (collisionCameraPrefab != null)
        {
            // Instantiate the camera
            GameObject cameraObj = Instantiate(collisionCameraPrefab);

            // Position the camera slightly above and behind the collision point
            cameraObj.transform.position = collisionPoint + cameraOffset;

            // Make the camera look at the collision point
            cameraObj.transform.LookAt(collisionPoint);
        }
        else
        {
            Debug.LogWarning("Collision camera prefab is not assigned.");
        }
    }

    private bool Perform3DConeRaycast()
    {
        bool targetDetected = false;

        // Cast rays in a 3D cone pattern
        for (int i = 0; i < rayCount; i++)
        {
            // Randomly generate a direction within the cone
            float angleHorizontal = Random.Range(-coneAngle / 2f, coneAngle / 2f);
            float angleVertical = Random.Range(-coneAngle / 2f, coneAngle / 2f);
            Quaternion rotation = Quaternion.Euler(angleVertical, angleHorizontal, 0);
            Vector3 rayDirection = rotation * transform.forward;

            // Perform the raycast
            Ray ray = new Ray(transform.position, rayDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {
                Debug.DrawRay(transform.position, rayDirection * raycastDistance, Color.red);

                // Check if the hit object is the target
                if (hit.collider.gameObject.tag == "Jet")
                {
                    targetDetected = true;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, rayDirection * raycastDistance, Color.green);
            }
        }
        return targetDetected;
    }

    private void RotateTowardsTarget()
    {
        // Calculate the direction to the target
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
    }
}
