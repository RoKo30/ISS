using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MissileMovement : MonoBehaviour
{
    [Header("Approx. FIM-92 Stinger Speeds")]
    [SerializeField] private float startSpeed = 50f;  
    [SerializeField] private float acceleration = 40; 
    [SerializeField] private float maxSpeed = 250f;      


    [Header("Turning Speeds (Degrees per second)")]
    [SerializeField] private float automaticTurningSpeed = 5f;  
    [SerializeField] private float manualTurningSpeed = 20f;    

    [Header("Detection")]
    [Tooltip("Max angle from forward to consider 'locked' on target.")]
    [SerializeField] private float maxLockAngle = 45f;    

    [SerializeField] private float maxDetectionDistance = 2000f; 

    [Header("Explosion & Camera")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject collisionCameraPrefab;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -10);

    [Header("Physical Properties")]
    [SerializeField] private float missileMass = 10f;

    private Rigidbody rb;
    private GameObject target;
    private bool manualMode = false;
    private float currentSpeed;

    private float pitch; 
    private float yaw;   

    private void Start()    
    {
        Debug.Log($"Missile Start: startSpeed={startSpeed}, acceleration={acceleration}, maxSpeed={maxSpeed}");

        Invoke(nameof(SelfDestruct), Random.Range(13f, 15f));
        rb = GetComponent<Rigidbody>();
        rb.mass = missileMass;
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        currentSpeed = startSpeed;

        target = GameObject.FindWithTag("Jet");
        if (target == null)
        {
            Debug.LogWarning("No jet found with 'Jet' tag!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            manualMode = !manualMode;
            Debug.Log(manualMode ? "Switched to Manual Mode" : "Switched to Automatic Mode");
        }

        pitch = 0f;
        yaw = 0f;

        if (Input.GetKey(KeyCode.W)) pitch = 1f;
        else if (Input.GetKey(KeyCode.S)) pitch = -1f;

        if (Input.GetKey(KeyCode.A)) yaw = -1f;
        else if (Input.GetKey(KeyCode.D)) yaw = 1f;
    }

    private void FixedUpdate()
    {
        currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.fixedDeltaTime, 0f, maxSpeed);

        rb.velocity = transform.forward * currentSpeed;

        if (manualMode)
        {
            HandleManualControl();
        }
        else if (target != null)
        {
            HandleAutomaticControl();
        }

        rb.MoveRotation(transform.rotation);
    }

private void HandleManualControl()
{
    float pitchAmount = pitch * manualTurningSpeed * Time.fixedDeltaTime;
    float yawAmount = yaw * manualTurningSpeed * Time.fixedDeltaTime;

    transform.Rotate(pitchAmount, yawAmount, 0f, Space.Self);
}



    private void HandleAutomaticControl()
    {
        Vector3 toTarget = target.transform.position - transform.position;
        if (toTarget.magnitude > maxDetectionDistance) return;

        float angleToTarget = Vector3.Angle(transform.forward, toTarget);
        if (angleToTarget > maxLockAngle) return; 

        Vector3 directionToTarget = toTarget.normalized;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            automaticTurningSpeed * Time.fixedDeltaTime
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Jet"))
        {
            Debug.Log("Missile hit the jet!");
            Vector3 hitPoint = collision.contacts[0].point;

            InstantiateExplosion(hitPoint);
            InstantiateCamera(hitPoint);

            Destroy(collision.gameObject); 
            Destroy(gameObject);           
        }
        else if (collision.transform.CompareTag("Terrain"))
        {
            Debug.Log("Missile hit the terrain!");
            Vector3 hitPoint = collision.contacts[0].point;

            InstantiateExplosion(hitPoint);
            InstantiateCamera(hitPoint);

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
            GameObject camObj = Instantiate(collisionCameraPrefab);
            camObj.transform.position = collisionPoint + cameraOffset;
            camObj.transform.LookAt(collisionPoint);
        }
        else
        {
            Debug.LogWarning("Collision camera prefab is not assigned.");
        }
    }

    private void SelfDestruct()
{
    Debug.Log("Missile self-destructed");

    InstantiateExplosion(transform.position);

    InstantiateCamera(transform.position);

    Destroy(gameObject);
}
}