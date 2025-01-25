using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MissileMovement : MonoBehaviour
{
    [SerializeField] private float startSpeed = 50f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float maxSpeed = 250f;
    [SerializeField] private float raycastDistance = 1000f;
    [SerializeField] private int rayCount = 30;
    [SerializeField] private float coneAngle = 30f;
    [SerializeField] private float automaticTurningSpeed = 5f;
    [SerializeField] private float manualTurningSpeed = 20f;
    
    [SerializeField] private float missileMass = 10.2f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject collisionCameraPrefab;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -10);

    private Rigidbody rb;
    private GameObject target;
    private bool manualMode = false;
    private float currentSpeed;
    private float pitch;
    private float yaw;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = missileMass;
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        currentSpeed = startSpeed;
        Debug.Log($"Missile Start: startSpeed= {startSpeed} ");

        Invoke(nameof(SelfDestruct), Random.Range(15f, 19f));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            manualMode = !manualMode;
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
        else
        {
            if (target == null)
            {
                Perform3DConeRaycast();
            }
            else
            {
                HandleAutomaticControl();
            }
        }
    }

    private void HandleManualControl()
    {
        float pitchAmount = pitch * manualTurningSpeed * Time.fixedDeltaTime;
        float yawAmount = yaw * manualTurningSpeed * Time.fixedDeltaTime;
        transform.Rotate(pitchAmount, yawAmount, 0f, Space.Self);
    }

    private void HandleAutomaticControl()
    {
        if (target == null) return;
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, automaticTurningSpeed * Time.fixedDeltaTime);
    }

    private void Perform3DConeRaycast()
    {
        for (int i = 0; i < rayCount; i++)
        {
            float angleH = Random.Range(-coneAngle / 2f, coneAngle / 2f);
            float angleV = Random.Range(-coneAngle / 2f, coneAngle / 2f);
            Quaternion rotation = Quaternion.Euler(angleV, angleH, 0f);
            Vector3 rayDirection = rotation * transform.forward;

            Ray ray = new Ray(transform.position, rayDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {
                Debug.DrawRay(transform.position, rayDirection * raycastDistance, Color.red);
                if (hit.transform.CompareTag("Jet"))
                {
                    target = hit.transform.gameObject;
                    break;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, rayDirection * raycastDistance, Color.green);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Jet"))
        {
            InstantiateExplosion(collision.contacts[0].point);
            InstantiateCamera(collision.contacts[0].point);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.transform.CompareTag("Terrain"))
        {
            InstantiateExplosion(collision.contacts[0].point);
            InstantiateCamera(collision.contacts[0].point);
            Destroy(gameObject);
        }
    }

    private void InstantiateExplosion(Vector3 position)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, position, Quaternion.identity);
        }
    }

    private void InstantiateCamera(Vector3 collisionPoint)
    {
        if (collisionCameraPrefab != null)
        {
            GameObject cameraObj = Instantiate(collisionCameraPrefab);
            cameraObj.transform.position = collisionPoint + cameraOffset;
            cameraObj.transform.LookAt(collisionPoint);
        }
    }

    private void SelfDestruct()
    {
        InstantiateExplosion(transform.position);
        InstantiateCamera(transform.position);
        Destroy(gameObject);
    }
}
