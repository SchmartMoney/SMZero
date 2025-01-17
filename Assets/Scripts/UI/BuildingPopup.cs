using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UltimateClean;

namespace SMZero
{
    public class BuildingPopup : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject backgroundPanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image buildingImage;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject characterSlotsContainer;
        [SerializeField] private CleanButton closeButton;
        [SerializeField] private CleanButton collectButton;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Character Slots")]
        [SerializeField] private CharacterSlotUI[] characterSlots;
        
        private Building currentBuilding;
        private bool isInitialized = false;
        
        private void Awake()
        {
            if (isInitialized) return;
            
            ValidateReferences();
            SetupListeners();
            Hide(); // Initial hide
            
            // Start periodic timer update
            InvokeRepeating("UpdateProductionTimer", 0f, 1f);
            
            isInitialized = true;
        }

        private void ValidateReferences()
        {
            if (backgroundPanel == null) Debug.LogError($"[{gameObject.name}] Background panel is missing!");
            if (nameText == null) Debug.LogError($"[{gameObject.name}] Name text is missing!");
            if (buildingImage == null) Debug.LogError($"[{gameObject.name}] Building image is missing!");
            if (timerText == null) Debug.LogError($"[{gameObject.name}] Timer text is missing!");
            if (characterSlotsContainer == null) Debug.LogError($"[{gameObject.name}] Character slots container is missing!");
            if (closeButton == null) Debug.LogError($"[{gameObject.name}] Close button is missing!");
            if (collectButton == null) Debug.LogError($"[{gameObject.name}] Collect button is missing!");
            if (canvasGroup == null) Debug.LogError($"[{gameObject.name}] Canvas group is missing!");
            
            if (characterSlots == null || characterSlots.Length == 0)
            {
                Debug.LogError($"[{gameObject.name}] Character slots array is empty!");
                // Try to find character slots in children
                characterSlots = characterSlotsContainer?.GetComponentsInChildren<CharacterSlotUI>(true);
                if (characterSlots == null || characterSlots.Length == 0)
                {
                    Debug.LogError($"[{gameObject.name}] Could not find any CharacterSlotUI components in children!");
                }
                else
                {
                    Debug.Log($"[{gameObject.name}] Found {characterSlots.Length} character slots in children");
                }
            }
        }

        private void SetupListeners()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseClicked);
            }
            
            if (collectButton != null)
            {
                collectButton.onClick.RemoveAllListeners();
                collectButton.onClick.AddListener(OnCollectClicked);
            }
        }
        
        public void Show(Building building)
        {
            Debug.Log($"Showing building popup for {building?.BuildingId ?? "null"}");
            if (building == null)
            {
                Debug.LogError("Cannot show popup: building is null");
                return;
            }

            if (!isInitialized)
            {
                ValidateReferences();
                SetupListeners();
                isInitialized = true;
            }

            currentBuilding = building;
            
            // Update UI
            if (nameText != null) nameText.text = building.BuildingId;
            if (buildingImage != null) buildingImage.sprite = building.GetBuildingSprite();
            
            // Show the UI
            if (backgroundPanel != null)
            {
                backgroundPanel.SetActive(true);
            }
            
            // Only affect this popup's canvas group
            var popupCanvasGroup = backgroundPanel?.GetComponent<CanvasGroup>();
            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = 1;
                popupCanvasGroup.interactable = true;
                popupCanvasGroup.blocksRaycasts = true;
            }

            UpdateCharacterSlots();
            UpdateProductionTimer();
            
            Debug.Log("Building popup shown successfully");
        }
        
        public void Hide()
        {
            Debug.Log("Hiding building popup");
            currentBuilding = null;
            
            // Only hide the popup panel, not the entire UI
            if (backgroundPanel != null)
            {
                backgroundPanel.SetActive(false);
            }
            
            // Only affect this popup's canvas group
            var popupCanvasGroup = backgroundPanel?.GetComponent<CanvasGroup>();
            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = 0;
                popupCanvasGroup.interactable = false;
                popupCanvasGroup.blocksRaycasts = false;
            }
        }
        
        private void UpdateProductionTimer()
        {
            if (currentBuilding == null || timerText == null || collectButton == null) return;

            if (!currentBuilding.IsProducing)
            {
                timerText.gameObject.SetActive(true);
                timerText.text = "No Production";
                collectButton.gameObject.SetActive(false);
                return;
            }

            float remainingTime = currentBuilding.GetRemainingProductionTime();
            bool isComplete = remainingTime <= 0;

            // Show/hide UI elements based on production state
            timerText.gameObject.SetActive(!isComplete);
            collectButton.gameObject.SetActive(isComplete);
            
            if (!isComplete)
            {
                timerText.text = $"Production Time: {FormatTime(remainingTime)}";
            }
            
            collectButton.interactable = isComplete;
        }
        
        private void UpdateCharacterSlots()
        {
            if (characterSlots == null || characterSlots.Length == 0)
            {
                Debug.LogError($"[{gameObject.name}] Cannot update character slots: no slots found");
                return;
            }

            var marketplaceManager = MarketplaceManager.Instance;
            if (marketplaceManager == null)
            {
                Debug.LogError("Cannot update character slots: MarketplaceManager not found");
                return;
            }

            // Get owned character NFTs
            var ownedCharacters = marketplaceManager.GetOwnedCharacterNFTs();
            Debug.Log($"Found {ownedCharacters?.Count ?? 0} owned characters");

            // Update each slot
            for (int i = 0; i < characterSlots.Length; i++)
            {
                var slot = characterSlots[i];
                if (slot != null)
                {
                    // Set up the slot with character info
                    string characterName = "";
                    string rarity = "";
                    bool isInteractable = false;

                    switch (i)
                    {
                        case 0:
                            characterName = "Richard";
                            rarity = "Common";
                            isInteractable = ownedCharacters?.Any(c => c.Id == "richard-nft-001") ?? false;
                            break;
                        case 1:
                            characterName = "Emily";
                            rarity = "Rare";
                            isInteractable = ownedCharacters?.Any(c => c.Id == "emily-nft-001") ?? false;
                            break;
                        case 2:
                            characterName = "Jake";
                            rarity = "Epic";
                            isInteractable = ownedCharacters?.Any(c => c.Id == "jake-nft-001") ?? false;
                            break;
                    }

                    slot.SetCharacter(characterName, rarity, isInteractable);
                    Debug.Log($"Updated character slot {i}: {characterName} (Interactable: {isInteractable})");
                }
                else
                {
                    Debug.LogError($"[{gameObject.name}] Character slot at index {i} is null!");
                }
            }
        }
        
        private string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60);
            int remainingSeconds = Mathf.FloorToInt(seconds % 60);
            return $"{minutes:00}:{remainingSeconds:00}";
        }
        
        private void OnCloseClicked()
        {
            Hide();
        }
        
        private void OnCollectClicked()
        {
            if (currentBuilding != null)
            {
                currentBuilding.CollectAssets();
                UpdateProductionTimer();
            }
        }
        
        private void OnDestroy()
        {
            if (closeButton != null) closeButton.onClick.RemoveAllListeners();
            if (collectButton != null) collectButton.onClick.RemoveAllListeners();
            CancelInvoke("UpdateProductionTimer");
        }

        public void SpeedUp()
        {
            if (currentBuilding == null) return;

            // Get current state
            var gameState = GameStateManager.Instance?.GetCurrentState();
            if (gameState?.ZonesState?.ZonesList != null)
            {
                var zoneState = gameState.ZonesState.ZonesList.Find(z => z.Id == currentBuilding.ZoneId);
                if (zoneState != null)
                {
                    var buildingState = zoneState.Buildings.Find(b => b.Id == currentBuilding.BuildingId);
                    if (buildingState?.Production != null)
                    {
                        // Set end time to 5 seconds from now
                        var now = DateTimeOffset.UtcNow;
                        var newEndTime = now.AddSeconds(5);
                        
                        // Set start time to make it appear as if production has been running for 59m55s
                        var newStartTime = newEndTime.AddSeconds(-3600); // 1 hour ago
                        
                        currentBuilding.SpeedUpProduction();

                        buildingState.Production.EndTimestamp = newEndTime.ToUnixTimeSeconds();
                        buildingState.Production.StartTimestamp = newStartTime.ToUnixTimeSeconds();
                        
                        // Update the game state
                        GameStateManager.Instance.ExportGameState();
                    }
                }
            }
        }
    }
} 