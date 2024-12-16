using UnityEngine;

public class CameraPanZoomDoubleClick : MonoBehaviour
{
    public float panSpeed = 0.5f;          // Speed of panning
    public Vector2 minBounds;             // Minimum X and Y bounds
    public Vector2 maxBounds;             // Maximum X and Y bounds

    public float zoomInSize = 5f;         // Orthographic size when zoomed in
    public float zoomOutSize = 10f;       // Orthographic size when zoomed out
    public float zoomSpeed = 5f;          // Speed of zooming
    private bool isZoomedIn = false;      // Track zoom state

    private Vector3 dragOrigin;           // Store the position where dragging starts
    private float lastClickTime = 0f;     // Track the time of the last click
    private float doubleClickTime = 0.3f; // Max time between clicks to count as a double-click

    void Update()
    {
        HandlePanning();
        HandleDoubleClickZoom();
    }

    private void HandlePanning()
    {
        // Detect left mouse button press
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition; // Save the starting point
        }

        // Detect left mouse button hold and drag
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(dragOrigin) - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            difference.z = 0; // Ensure no movement on the Z-axis (for 2D games)
            transform.position += difference; // Move the camera
            dragOrigin = Input.mousePosition; // Update drag origin

            // Clamp the camera position within bounds
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                transform.position.z
            );
        }
    }

    private void HandleDoubleClickZoom()
    {
        // Detect mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the click is within the double-click timeframe
            if (Time.time - lastClickTime < doubleClickTime)
            {
                if (!isZoomedIn)
                {
                    // Center and zoom in on the middle of the screen
                    Vector3 screenCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
                    StartCoroutine(SmoothCenterAndZoom(screenCenter, zoomInSize));
                }
                else
                {
                    // Zoom out, centering on the camera's current position
                    StartCoroutine(SmoothCenterAndZoom(transform.position, zoomOutSize));
                }
            }

            // Update the last click time
            lastClickTime = Time.time;
        }
    }

    private System.Collections.IEnumerator SmoothCenterAndZoom(Vector3 targetPosition, float targetZoomSize)
    {
        float startSize = Camera.main.orthographicSize;
        Vector3 startPosition = transform.position;

        // Clamp the target position within bounds
        Vector3 clampedTargetPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y),
            transform.position.z
        );

        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * zoomSpeed;

            // Smoothly interpolate camera position and orthographic size
            Camera.main.orthographicSize = Mathf.Lerp(startSize, targetZoomSize, elapsed);
            transform.position = Vector3.Lerp(startPosition, clampedTargetPosition, elapsed);
            yield return null;
        }

        Camera.main.orthographicSize = targetZoomSize;
        transform.position = clampedTargetPosition;

        // Toggle zoom state
        isZoomedIn = targetZoomSize == zoomInSize;
    }
}
