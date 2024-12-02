using UnityEngine;

public class RTSLikeCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float panSpeed = 20f;             // Speed of camera panning
    public float panBorderThickness = 10f;   // Thickness for edge panning (in pixels)
    public Vector2 panLimit = new Vector2(5000, 5000); // Adjusted for large map size

    [Header("Zoom Settings")]
    public float zoomSpeed = 500f;           // Adjusted zoom speed for large maps

    [Header("Height Settings")]
    public float minHeight = 10f;            // Minimum height above ground
    public float maxHeight = 1000f;          // Maximum height above ground

    [Header("Rotation Settings")]
    public float rotateSpeed = 100f;         // Adjusted rotation speed
    public float middleMouseRotateSensitivity = 0.5f; // Adjusted sensitivity for large maps
    public Vector3 rotationCenter;           // Center point for rotation

    [Header("Pitch Settings")]
    public float minPitchAngle = 10f;        // Minimum pitch angle (looking down)
    public float maxPitchAngle = 80f;        // Maximum pitch angle (looking up)

    [Header("Mouse Settings")]
    public bool enableEdgePanning = true;    // Enable panning when mouse is near screen edges

    private Vector3 targetPosition;
    private Vector3 lastMousePosition;
    private bool isRotating = false;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isRotating)
        {
            HandleMovement();
        }

        HandleZoom();
        HandleRotation();
        HandleMiddleMouseRotation();

        // Smoothly move the camera to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panSpeed);
    }

    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        // Keyboard Input
        if (Input.GetKey(KeyCode.W) || (enableEdgePanning && Input.mousePosition.y >= Screen.height - panBorderThickness))
            move.z += 1;
        if (Input.GetKey(KeyCode.S) || (enableEdgePanning && Input.mousePosition.y <= panBorderThickness))
            move.z -= 1;
        if (Input.GetKey(KeyCode.A) || (enableEdgePanning && Input.mousePosition.x <= panBorderThickness))
            move.x -= 1;
        if (Input.GetKey(KeyCode.D) || (enableEdgePanning && Input.mousePosition.x >= Screen.width - panBorderThickness))
            move.x += 1;

        move = move.normalized * panSpeed * Time.deltaTime;
        targetPosition += Quaternion.Euler(0, transform.eulerAngles.y, 0) * move;

        // Clamp the target position within boundaries
        ClampPosition();
    }

    private void HandleZoom()
    {
        // Zoom based on scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            // Calculate zoom amount
            float zoomAmount = scroll * zoomSpeed * Time.deltaTime;

            // Move camera along its forward vector
            targetPosition += transform.forward * zoomAmount;

            // Clamp the target position within boundaries
            ClampPosition();
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            SetRotationCenter();
            RotateAroundCenter(-rotateSpeed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            SetRotationCenter();
            RotateAroundCenter(rotateSpeed * Time.deltaTime, 0f);
        }
    }

    private void HandleMiddleMouseRotation()
    {
        if (Input.GetMouseButtonDown(2))
        {
            // Store the initial mouse position when the middle mouse button is pressed
            lastMousePosition = Input.mousePosition;
            isRotating = true; // Disable panning while rotating

            // Set rotation center dynamically
            SetRotationCenter();
        }

        if (Input.GetMouseButton(2))
        {
            // Calculate the drag delta
            Vector3 delta = Input.mousePosition - lastMousePosition;

            // Rotate around the center point based on the mouse movement
            float rotationAngleY = delta.x * middleMouseRotateSensitivity; // Yaw (left/right)
            float rotationAngleX = -delta.y * middleMouseRotateSensitivity; // Pitch (up/down)

            RotateAroundCenter(rotationAngleY, rotationAngleX);

            // Update the last mouse position
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isRotating = false; // Re-enable panning when middle mouse button is released
        }
    }

    private void RotateAroundCenter(float angleY, float angleX)
    {
        // Get the current pitch angle
        float currentPitch = transform.eulerAngles.x;
        if (currentPitch > 180f)
            currentPitch -= 360f;

        // Calculate what the new pitch angle would be after rotation
        float newPitch = currentPitch + angleX;

        // Clamp the new pitch angle
        float clampedPitch = Mathf.Clamp(newPitch, minPitchAngle, maxPitchAngle);

        // Adjust angleX so that it doesn't exceed the clamped values
        float pitchDelta = clampedPitch - currentPitch;

        // Rotate around the rotationCenter point for yaw (Y-axis)
        transform.RotateAround(rotationCenter, Vector3.up, angleY);

        // Rotate around the camera's local X-axis for pitch
        transform.RotateAround(rotationCenter, transform.right, pitchDelta);

        // Reset roll to zero to prevent unintended rolling
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.z = 0f;
        transform.eulerAngles = eulerAngles;

        // Update targetPosition to match the new position
        targetPosition = transform.position;

        // Clamp the target position within boundaries
        ClampPosition();

        // Apply the clamped position
        transform.position = targetPosition;
    }

    private void SetRotationCenter()
    {
        Ray ray;
        RaycastHit hit;

        // Raycast from the camera's position forward to find a rotation center
        ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            rotationCenter = hit.point;
        }
        else
        {
            // Raycast from the camera's position downward to find the ground
            ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit))
            {
                rotationCenter = hit.point;
            }
            else
            {
                // If no ground is found, set rotation center ahead of the camera based on height
                float dynamicForwardMultiplier = Mathf.Clamp(transform.position.y * 2f, 100f, 1000f);
                rotationCenter = transform.position + transform.forward * dynamicForwardMultiplier;
            }
        }
    }

    private void ClampPosition()
    {
        // Enforce movement limits
        targetPosition.x = Mathf.Clamp(targetPosition.x, 0, panLimit.x);
        targetPosition.z = Mathf.Clamp(targetPosition.z, 0, panLimit.y);

        // Clamp camera's height
        targetPosition.y = Mathf.Clamp(targetPosition.y, minHeight, maxHeight);
    }
}
