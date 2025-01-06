using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 30f;  // Increased for wider view
    [SerializeField] private float maxZoom = 10f;  // This is closer zoom (smaller number)
    [SerializeField] private float zoomSpeed = 5f; // Increased for better mouse wheel response
    [SerializeField] private float zoomSmoothness = 10f;

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 7f;
    [SerializeField] private float panSmoothness = 10f;
    [SerializeField] private float panLimitOffset = 100f; // Increased for larger map

    private Camera mainCamera;
    private Vector3 targetPosition;
    private float targetZoom;
    private Vector3 lastPanPosition;
    private bool isPanning = false;
    private float currentZoomVelocity;
    private Vector3 currentPanVelocity;
    private Vector3 initialPosition;
    private Vector2 panLimitMin;
    private Vector2 panLimitMax;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        // Set initial zoom values based on current camera
        float currentSize = mainCamera.orthographicSize;
        minZoom = currentSize + 10f;
        maxZoom = currentSize * 0.3f;
        targetZoom = currentSize;
    }

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition;

        // Set pan limits based on initial position
        panLimitMin = new Vector2(
            initialPosition.x - panLimitOffset,
            initialPosition.y - panLimitOffset
        );
        panLimitMax = new Vector2(
            initialPosition.x + panLimitOffset,
            initialPosition.y + panLimitOffset
        );
    }

    private void Update()
    {
        HandleMouseInput();
        HandleTouchInput();
        UpdateCameraTransform();
    }

    private void HandleMouseInput()
    {
        // Mouse wheel zoom - reversed direction and increased sensitivity
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            float zoomDelta = scrollDelta * zoomSpeed;
            float newZoom = Mathf.Clamp(mainCamera.orthographicSize - zoomDelta, maxZoom, minZoom);
            targetZoom = newZoom;
            Debug.Log($"Current: {mainCamera.orthographicSize}, Target: {targetZoom}, Delta: {zoomDelta}");
        }

        // Mouse pan
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            isPanning = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 currentPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = lastPanPosition - currentPosition;
            
            float orthographicSize = mainCamera.orthographicSize;
            Vector3 move = new Vector3(
                direction.x * panSpeed * orthographicSize,
                direction.y * panSpeed * orthographicSize,
                0
            );

            targetPosition += move;
            lastPanPosition = currentPosition;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Get touch positions from previous frame
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            // Get the magnitudes
            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // Get the difference in magnitudes
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Change zoom based on the pinch gesture
            float newZoom = Mathf.Clamp(mainCamera.orthographicSize + deltaMagnitudeDiff * 0.01f * zoomSpeed, maxZoom, minZoom);
            targetZoom = newZoom;

            // Handle two finger pan
            Vector2 touch0Delta = touch0.deltaPosition;
            Vector2 touch1Delta = touch1.deltaPosition;
            Vector2 averageDelta = (touch0Delta + touch1Delta) * 0.5f;

            Vector3 move = new Vector3(
                averageDelta.x * panSpeed * 0.001f * mainCamera.orthographicSize,
                averageDelta.y * panSpeed * 0.001f * mainCamera.orthographicSize,
                0
            );

            targetPosition -= move;
        }
    }

    private void UpdateCameraTransform()
    {
        // Clamp target position within bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, panLimitMin.x, panLimitMax.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, panLimitMin.y, panLimitMax.y);
        targetPosition.z = initialPosition.z;

        // Smoothly update camera position and zoom
        float newSize = Mathf.SmoothDamp(
            mainCamera.orthographicSize,
            targetZoom,
            ref currentZoomVelocity,
            1f / zoomSmoothness
        );
        mainCamera.orthographicSize = newSize;

        // Update position while maintaining Z
        Vector3 newPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentPanVelocity,
            1f / panSmoothness
        );
        newPosition.z = initialPosition.z;
        transform.position = newPosition;
    }
} 