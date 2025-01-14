using UnityEngine;
using UnityEngine.SceneManagement;
using Exoa.Touch;
using Exoa.Common;
using SMZero;

namespace SMZero
{
    [RequireComponent(typeof(LocationHighlight))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(TouchSelectable))]
    public class LocationInteraction : TouchSelectableBehaviour
    {
        [Header("Location Settings")]
        [SerializeField] private LocationData locationData;
        [SerializeField] private LocationUIData uiData;
        
        [Header("State Settings")]
        [SerializeField] private bool isActive = false;

        [Header("Colors")]
        [SerializeField] private Color activeColor = Color.yellow;
        [SerializeField] private Color inactiveColor = Color.grey;

        [Header("UI References")]
        [SerializeField] private GameObject marketplaceCanvas;

        private LocationHighlight highlight;
        private LocationPopupUI popupUI;
        private TouchSelectable touchSelectable;
        private MarketplaceManager marketplaceManager;
        private GameStateManager gameStateManager;

        private void Awake()
        {
            highlight = GetComponent<LocationHighlight>();
            popupUI = FindObjectOfType<LocationPopupUI>();
            touchSelectable = GetComponent<TouchSelectable>();
            marketplaceManager = MarketplaceManager.Instance;
            gameStateManager = GameStateManager.Instance;

            if (popupUI == null)
            {
                Debug.LogError("LocationPopupUI not found in scene!");
            }

            // Ensure we have required components
            if (GetComponent<Collider>() == null)
            {
                Debug.LogError($"Location {gameObject.name} needs a Collider component!");
            }

            // Update highlight based on both interactable and active states
            UpdateHighlightState();
        }

        private void UpdateHighlightState()
        {
            // Location is only truly interactable if it's both active and marked as interactable
            bool canInteract = isActive && locationData.isInteractable;

            // Set color based on interactable state
            highlight.SetHighlightColor(canInteract ? activeColor : inactiveColor);
            highlight.SetInteractable(canInteract);
        }

        public void SetActive(bool active)
        {
            isActive = active;
            UpdateHighlightState();
        }

        // Override OnSelected from TouchSelectableBehaviour
        protected override void OnSelected(TouchSelect select)
        {
            if (Application.isMobilePlatform)
            {
                HandleInteraction();
            }
        }

        // Keep OnMouseDown for PC/Editor testing
        private void OnMouseDown()
        {
            if (!Application.isMobilePlatform)
            {
                HandleInteraction();
            }
        }

        private void HandleInteraction()
        {
            // Check both active state and interactable state
            bool canInteract = isActive && locationData.isInteractable;
            if (!canInteract)
            {
                Debug.Log($"Location {locationData.displayName} is not available for interaction");
                return;
            }

            Debug.Log($"Showing popup for location: {locationData.displayName}");
            
            // If this is a marketplace building, show the marketplace UI
            if (locationData.isMarketplace && marketplaceCanvas != null)
            {
                marketplaceCanvas.SetActive(true);
                return;
            }

            // Otherwise show the regular location popup
            if (popupUI != null)
            {
                popupUI.Show(locationData, uiData, OnEnterLocation, OnLeaveLocation);
            }
        }

        private void OnEnterLocation()
        {
            Debug.Log($"Entering location: {locationData.displayName}");
            
            // Save current state before scene transition
            if (gameStateManager != null && marketplaceManager != null)
            {
                SaveCurrentState();
            }

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

        private void SaveCurrentState()
        {
            // Save current state before scene transition
            if (gameStateManager != null)
            {
                gameStateManager.ExportGameState();
            }
        }

        private void OnLeaveLocation()
        {
            // Save current state before leaving
            GameStateManager.Instance.ExportGameState();
            SceneManager.LoadScene("MainScene");
        }

        private void Hide()
        {
            if (popupUI != null)
            {
                popupUI.Hide();
            }
        }
    }
} 