using UnityEngine;

public class RTSLikeCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float panSpeed = 20f;        // Speed of camera panning
    public float panBorderThickness = 10f; // Thickness for edge panning (in pixels)
    public Vector2 panLimit = new Vector2(50, 50); // X and Z limits for movement

    [Header("Zoom Settings")]
    public float zoomSpeed = 50f;      // Speed of zooming
    public float minZoom = 10f;        // Minimum height of the camera
    public float maxZoom = 100f;       // Maximum height of the camera

    [Header("Rotation Settings")]
    public float rotateSpeed = 50f;    // Speed of rotation
    public float middleMouseRotateSensitivity = 0.2f; // Sensitivity for middle mouse rotation
    public Transform focusPoint;       // Optional focal point for rotation (can be null)

    public float minPitch = -30f;      // Minimum pitch (rotation around the X-axis)
    public float maxPitch = 60f;       // Maximum pitch (rotation around the X-axis)

    [Header("Mouse Settings")]
    public bool enableEdgePanning = true; // Enable panning when mouse is near screen edges

    private Vector3 targetPosition;
    private float currentZoom;
    private Vector3 lastMousePosition;
    private bool isRotating = false;

    private void Start()
    {
        targetPosition = transform.position;
        currentZoom = transform.position.y;
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

        // Enforce movement limits
        targetPosition.x = Mathf.Clamp(targetPosition.x, 0, panLimit.x);
        targetPosition.z = Mathf.Clamp(targetPosition.z, 0, panLimit.y);
    }

    private void HandleZoom()
    {
        // Zoom based on scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed * Time.deltaTime;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        targetPosition.y = Mathf.Lerp(transform.position.y, currentZoom, Time.deltaTime * zoomSpeed);
    }

    private void HandleRotation()
    {
        // Rotate with Q and E keys
        if (Input.GetKey(KeyCode.Q))
        {
            RotateCamera(-rotateSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            RotateCamera(rotateSpeed);
        }
    }

    private void RotateCamera(float angle)
    {
        if (focusPoint != null)
        {
            // Rotate around the focus point
            transform.RotateAround(focusPoint.position, Vector3.up, angle * Time.deltaTime);
        }
        else
        {
            // Rotate in place
            transform.Rotate(Vector3.up, angle * Time.deltaTime, Space.World);
        }
    }

    private void HandleMiddleMouseRotation()
    {
        if (Input.GetMouseButtonDown(2))
        {
            // Store the initial mouse position when the middle mouse button is pressed
            lastMousePosition = Input.mousePosition;
            isRotating = true; // Disable panning while rotating
        }

        if (Input.GetMouseButton(2))
        {
            // Calculate the drag delta
            Vector3 delta = Input.mousePosition - lastMousePosition;

            // Rotate the camera based on the delta
            float rotationX = -delta.y * middleMouseRotateSensitivity;
            float rotationY = delta.x * middleMouseRotateSensitivity;

            // Apply rotation with limits
            Vector3 newEulerAngles = transform.eulerAngles + new Vector3(rotationX, rotationY, 0);

            // Clamp the pitch (X-axis rotation)
            newEulerAngles.x = Mathf.Clamp(newEulerAngles.x, minPitch, maxPitch);

            // Apply rotation
            transform.eulerAngles = newEulerAngles;

            // Update the last mouse position
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isRotating = false; // Re-enable panning when middle mouse button is released
        }
    }
}
