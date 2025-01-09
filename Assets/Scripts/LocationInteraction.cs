using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LocationHighlight))]
public class LocationInteraction : MonoBehaviour
{
    [Header("Location Settings")]
    [SerializeField] private LocationData locationData;
    [SerializeField] private LocationUIData uiData;
    
    [Header("State Settings")]
    [SerializeField] private bool isActive = false;  // New field for active state

    private LocationHighlight highlight;
    private LocationPopupUI popupUI;

    private void Awake()
    {
        highlight = GetComponent<LocationHighlight>();
        popupUI = FindObjectOfType<LocationPopupUI>();

        if (popupUI == null)
        {
            Debug.LogError("LocationPopupUI not found in scene!");
        }

        // Update highlight based on both interactable and active states
        UpdateHighlightState();
    }

    private void UpdateHighlightState()
    {
        // Location is only truly interactable if it's both active and marked as interactable
        bool canInteract = isActive && locationData.isInteractable;
        highlight.SetInteractable(canInteract);
        
        // Set color based on active state
        highlight.SetHighlightColor(isActive ? Color.yellow : Color.grey);
    }

    public void SetActive(bool active)
    {
        isActive = active;
        UpdateHighlightState();
    }

    private void OnMouseDown()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        // Check both active state and interactable state
        if (!isActive || !locationData.isInteractable)
        {
            Debug.Log($"Location {locationData.displayName} is not available for interaction");
            return;
        }

        Debug.Log($"Showing popup for location: {locationData.displayName}");
        if (popupUI != null)
        {
            popupUI.Show(locationData, uiData, OnEnterLocation, OnLeaveLocation);
        }
    }

    private void OnEnterLocation()
    {
        Debug.Log($"Entering location: {locationData.displayName}");
        if (locationData.ValidateScenePath())
        {
            SceneManager.LoadScene(locationData.sceneToLoad);
        }
        else
        {
            Debug.LogError($"Cannot enter location: {locationData.displayName} - Invalid scene path");
            Hide();
        }
    }

    private void OnLeaveLocation()
    {
        Debug.Log($"Leaving location: {locationData.displayName}");
        if (popupUI != null)
        {
            popupUI.Hide();
        }
    }

    private void Hide()
    {
        if (popupUI != null)
        {
            popupUI.Hide();
        }
    }
} 