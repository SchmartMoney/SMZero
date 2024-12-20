using UnityEngine;

public class CameraPanZoomDoubleClick : MonoBehaviour
{
    public float panSpeed = 0.5f;             // Speed of panning
    public Vector2 minBounds;                // Minimum X and Y bounds
    public Vector2 maxBounds;                // Maximum X and Y bounds

    public float zoomInSize = 5f;            // Orthographic size when zoomed in
    public float zoomOutSize = 10f;          // Orthographic size when zoomed out
    public float zoomSpeed = 5f;             // Speed of zooming
    private bool isZoomedIn = false;         // Track zoom state

    private Vector3 dragOrigin;              // Store the position where dragging starts
    private bool isDragging = false;         // Track if the user is dragging
    private Vector3 dragVelocity;            // Store the velocity of the drag
    private bool isDecelerating = false;     // Track whether deceleration is happening
    public float decelerationRate = 5f;      // Rate at which the camera slows down

    private bool hasInteracted = false;      // Ensure interaction only happens once after release
    public GameObject menu;                  // Reference to the menu GameObject

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
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = false;
            isDecelerating = false; // Stop deceleration if new input begins
            dragVelocity = Vector3.zero; // Reset velocity
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 previousPosition = Camera.main.ScreenToWorldPoint(dragOrigin);
            Vector3 difference = previousPosition - currentPosition;

            if (difference.magnitude > 0.01f) // Threshold to detect dragging
            {
                isDragging = true;
            }

            difference.z = 0; // Ensure no movement on the Z-axis (for 2D games)
            transform.position += difference; // Move the camera
            dragOrigin = Input.mousePosition; // Update drag origin

            // Clamp the camera position within bounds
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                transform.position.z
            );

            // Store velocity for deceleration
            dragVelocity = difference / Time.deltaTime; // Velocity = distance / time
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                isDragging = false;
                isDecelerating = true; // Start deceleration
            }
            else if (!hasInteracted) // Handle clicks without dragging
            {
                hasInteracted = true;

                if (menu != null)
                {
                    menu.SetActive(!menu.activeSelf); // Toggle menu visibility
                }
            }
        }
    }

    private void ApplyDeceleration()
    {
        if (dragVelocity.magnitude > 0.1f) // Stop deceleration when velocity is minimal
        {
            dragVelocity = Vector3.Lerp(dragVelocity, Vector3.zero, Time.deltaTime * decelerationRate);

            // Apply the velocity to the camera position
            transform.position += dragVelocity * Time.deltaTime;

            // Clamp the camera position within bounds
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                transform.position.z
            );
        }
        else
        {
            isDecelerating = false; // Stop deceleration
            dragVelocity = Vector3.zero; // Reset velocity
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
                isDecelerating = false; // Stop deceleration if new input begins
                dragVelocity = Vector3.zero; // Reset velocity
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(touch.position);
                Vector3 previousPosition = Camera.main.ScreenToWorldPoint(dragOrigin);
                Vector3 difference = previousPosition - currentPosition;

                if (difference.magnitude > 0.01f) // Threshold to detect dragging
                {
                    isDragging = true;
                }

                difference.z = 0; // Ensure no movement on the Z-axis (for 2D games)
                transform.position += difference; // Move the camera
                dragOrigin = touch.position; // Update drag origin

                // Clamp the camera position within bounds
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                    Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                    transform.position.z
                );

                // Store velocity for deceleration
                dragVelocity = difference / Time.deltaTime; // Velocity = distance / time
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                    isDragging = false;
                    isDecelerating = true; // Start deceleration
                }
            }
        }
        else if (Input.touchCount == 2) // Handle pinch-to-zoom
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevMagnitude = (touch1PrevPos - touch2PrevPos).magnitude;
            float currentMagnitude = (touch1.position - touch2.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Camera.main.orthographicSize -= difference * 0.01f; // Adjust sensitivity as needed
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoomInSize, zoomOutSize);
        }
    }
}
