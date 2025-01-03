using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 5f; // Speed of rotation for both axes

    [SerializeField]
    private float verticalLookLimit = 60f; // Limit for looking up and down

    [SerializeField]
    private Transform cameraTransform; // Reference to the camera (or head) for vertical rotation

    [SerializeField]
    private GameObject prefabToInstantiate; // Prefab to instantiate when shooting

    [SerializeField]
    private Transform spawnPoint; // Point where the prefab is instantiated

    private float verticalRotation = 0f; // Track the up/down rotation angle

    void Update()
    {
        RotateWithMouse();
        HandleShooting();
    }

    private void RotateWithMouse()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Horizontal rotation (Y-axis)
        transform.Rotate(0, mouseX * rotationSpeed, 0);

        // Vertical rotation (X-axis)
        verticalRotation -= mouseY * rotationSpeed; // Subtract to invert mouse Y-axis for natural feel
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);

        // Apply vertical rotation to the camera or head
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            if (prefabToInstantiate != null && spawnPoint != null && cameraTransform != null)
            {
                // Calculate the rotation based on the camera's forward direction
                Quaternion spawnRotation = Quaternion.LookRotation(cameraTransform.forward);

                // Instantiate the prefab with the calculated rotation
                Instantiate(prefabToInstantiate, spawnPoint.position, spawnRotation);
            }
            else
            {
                Debug.LogWarning("Prefab, spawn point, or camera transform not assigned!");
            }
        }
    }

}