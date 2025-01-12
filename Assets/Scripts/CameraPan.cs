using UnityEngine;

public class CameraPanZoomDoubleClick : MonoBehaviour
{
    public float panSpeed = 0.5f;             // Speed of panning
    [Header("Bounds Settings")]
    public Vector2 minBounds = new Vector2(-90f, 85f);   // Minimum X and Z bounds
    public Vector2 maxBounds = new Vector2(287f, 431f);  // Maximum X and Z bounds

    [Header("Zoom Settings")]
    public float minZoom = 20f;              // Minimum zoom distance (closest)
    public float maxZoom = 400f;             // Maximum zoom distance (farthest)
    public float zoomSpeed = 2f;             // Speed of zooming
    public float zoomSmoothness = 5f;        // How smooth the zoom should be
    private float targetZoom;                // Target zoom level
    private Camera mainCamera;               // Reference to the camera
    private float initialRotationX;          // Store initial X rotation

    [Header("Movement Settings")]
    public float heightFactorDivisor = 15f;  // Divisor for height-based movement scaling
    public float decelerationRate = 5f;      // Rate at which the camera slows down

    private Vector3 dragOrigin;              // Store the position where dragging starts
    private bool isDragging = false;         // Track if the user is dragging
    private Vector3 dragVelocity;            // Store the velocity of the drag
    private bool isDecelerating = false;     // Track whether deceleration is happening

    private bool hasInteracted = false;      // Ensure interaction only happens once after release
    public GameObject menu;                  // Reference to the menu GameObject

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Store initial rotation
        initialRotationX = transform.eulerAngles.x;

        // Initialize target zoom based on current camera position
        targetZoom = Vector3.Distance(transform.position, Vector3.zero);
    }

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }

        // Apply deceleration when not dragging
        if (isDecelerating)
        {
            ApplyDeceleration();
        }

        // Apply smooth zoom
        UpdateZoom();
    }

    private void UpdateZoom()
    {
        if (Mathf.Abs(Vector3.Distance(transform.position, Vector3.zero) - targetZoom) > 0.01f)
        {
            // Calculate direction from camera to target (keeping the same angle)
            Vector3 directionToTarget = transform.position.normalized;
            Vector3 targetPosition = directionToTarget * targetZoom;
            
            // Maintain the same height ratio based on camera angle
            float heightRatio = Mathf.Sin(initialRotationX * Mathf.Deg2Rad);
            targetPosition.y = targetZoom * heightRatio;

            // Smoothly move to target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomSmoothness);
        }
    }

    private void HandleMouseInput()
    {
        // Handle zooming with mouse wheel
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            float zoomDelta = scrollDelta * zoomSpeed * targetZoom;
            targetZoom = Mathf.Clamp(targetZoom - zoomDelta, minZoom, maxZoom);
        }

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = false;
            isDecelerating = false;
            dragVelocity = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPosition = GetWorldPosition(Input.mousePosition);
            Vector3 previousPosition = GetWorldPosition(dragOrigin);
            
            if (currentPosition != Vector3.zero && previousPosition != Vector3.zero)
            {
                Vector3 difference = previousPosition - currentPosition;

                if (difference.magnitude > 0.01f)
                {
                    isDragging = true;
                }

                // Scale the movement based on camera height
                float heightFactor = transform.position.y / heightFactorDivisor;
                difference *= heightFactor * panSpeed;

                // Move camera while maintaining its angle
                Vector3 movement = new Vector3(difference.x, 0, difference.z);
                transform.position += movement;

                dragOrigin = Input.mousePosition;
                ClampCameraPosition();
                dragVelocity = movement / Time.deltaTime;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                isDragging = false;
                isDecelerating = true;
            }
            else if (!hasInteracted)
            {
                hasInteracted = true;
                if (menu != null)
                {
                    menu.SetActive(!menu.activeSelf);
                }
            }
        }
    }

    private Vector3 GetWorldPosition(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;

        if (groundPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    private void ClampCameraPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.y, maxBounds.y);
        
        // Maintain Y position based on current zoom level and angle
        float heightRatio = Mathf.Sin(initialRotationX * Mathf.Deg2Rad);
        pos.y = Vector3.Distance(new Vector3(pos.x, 0, pos.z), Vector3.zero) * heightRatio;
        
        transform.position = pos;
    }

    private void ApplyDeceleration()
    {
        if (dragVelocity.magnitude > 0.1f)
        {
            dragVelocity = Vector3.Lerp(dragVelocity, Vector3.zero, Time.deltaTime * decelerationRate);
            transform.position += dragVelocity * Time.deltaTime;
            ClampCameraPosition();
        }
        else
        {
            isDecelerating = false;
            dragVelocity = Vector3.zero;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = touch.position;
                isDragging = false;
                isDecelerating = false;
                dragVelocity = Vector3.zero;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 currentPosition = GetWorldPosition(touch.position);
                Vector3 previousPosition = GetWorldPosition(dragOrigin);

                if (currentPosition != Vector3.zero && previousPosition != Vector3.zero)
                {
                    Vector3 difference = previousPosition - currentPosition;

                    if (difference.magnitude > 0.01f)
                    {
                        isDragging = true;
                    }

                    // Scale the movement based on camera height
                    float heightFactor = transform.position.y / heightFactorDivisor;
                    difference *= heightFactor * panSpeed;

                    // Move camera while maintaining its angle
                    Vector3 movement = new Vector3(difference.x, 0, difference.z);
                    transform.position += movement;

                    dragOrigin = touch.position;
                    ClampCameraPosition();
                    dragVelocity = movement / Time.deltaTime;
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                    isDragging = false;
                    isDecelerating = true;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevMagnitude = (touch1PrevPos - touch2PrevPos).magnitude;
            float currentMagnitude = (touch1.position - touch2.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            
            // Update target zoom based on pinch gesture
            float zoomDelta = difference * 0.01f * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom - zoomDelta * targetZoom, minZoom, maxZoom);
        }
    }
}
