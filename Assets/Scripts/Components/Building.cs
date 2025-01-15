using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Exoa.Cameras;
using Exoa.Events;
using Exoa.Touch;

namespace SMZero
{
    public class Building : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float baseProductionTime = 10f; // 10 seconds for testing
        [SerializeField] private int baseFortuneAmount = 100;
        [SerializeField] private string buildingId = "vault-avenue-001"; // Main Vault Avenue NFT
        [SerializeField] private string zoneId = "vault_avenue"; // Default to Vault Avenue zone
        [SerializeField] private Sprite buildingSprite; // Building sprite for UI

        [Header("Colors")]
        [SerializeField] private Color activeColor = Color.yellow;
        [SerializeField] private Color inactiveColor = Color.gray;
        
        private bool isActive;
        private List<CharacterNFT> stakedCharacters = new List<CharacterNFT>();
        private float currentProductionTime;
        private bool isProducing;
        private bool canCollect;
        private LocationHighlight locationHighlight;
        private GameStateManager gameStateManager;

        // Public properties for UI
        public bool IsActive => isActive;
        public bool IsProducing => isProducing;
        public bool CanCollect => canCollect;
        public float CurrentProductionTime => currentProductionTime;
        public float BaseProductionTime => baseProductionTime;
        public IReadOnlyList<CharacterNFT> StakedCharacters => stakedCharacters.AsReadOnly();
        public int BaseFortuneAmount => baseFortuneAmount;
        public string BuildingId => buildingId;
        public string ZoneId => zoneId;

        public List<string> GetStakedCharacterIds()
        {
            return stakedCharacters.Select(c => c.Id).ToList();
        }

        private void Awake()
        {
            locationHighlight = GetComponent<LocationHighlight>();
            gameStateManager = GameStateManager.Instance;
            
            if (locationHighlight == null)
            {
                Debug.LogError($"Building {gameObject.name} is missing LocationHighlight component!");
            }

            // Add collider for click detection if not present
            if (GetComponent<Collider>() == null)
            {
                var collider = gameObject.AddComponent<BoxCollider>();
                collider.isTrigger = true;
            }

            // Validate building ID
            ValidateBuildingId();

            LoadState();
        }

        private void ValidateBuildingId()
        {
            Debug.Log($"Building ID: {buildingId}");
            if (string.IsNullOrEmpty(buildingId))
            {
                Debug.LogError($"Building {gameObject.name} has no ID set!");
                return;
            }

            // Expected formats:
            // Main building: "vault-avenue-001"
            // Other buildings: "vault-avenue-002" through "vault-avenue-010"
            if (buildingId == "vault-avenue-001")
            {
                Debug.Log("This is the main Vault Avenue building");
            }
            else if (buildingId.StartsWith("vault-avenue-"))
            {
                string numberPart = buildingId.Replace("vault-avenue-", "");
                if (int.TryParse(numberPart, out int buildingNumber))
                {
                    if (buildingNumber >= 2 && buildingNumber <= 10)
                    {
                        Debug.Log($"This is Vault Avenue building #{buildingNumber}");
                    }
                    else
                    {
                        Debug.LogError($"Invalid building number: {buildingNumber}. Should be between 2 and 10.");
                    }
                }
                else
                {
                    Debug.LogError($"Invalid building ID format: {buildingId}");
                }
            }
            else
            {
                Debug.LogError($"Invalid building ID format: {buildingId}. Should be either 'vault-avenue-001' or 'vault-avenue-XXX' where XXX is 002-010");
            }
        }

        private void LoadState()
        {
            Debug.Log($"=== Loading Building State for {buildingId} ===");
            if (gameStateManager == null)
            {
                Debug.LogWarning("GameStateManager not found during building state load");
                return;
            }

            var gameState = gameStateManager.GetCurrentState();
            if (gameState?.ZonesState?.ZonesList != null)
            {
                var zoneState = gameState.ZonesState.ZonesList.Find(z => z.Id == zoneId);
                Debug.Log($"Found zone state? {zoneState != null}");
                
                if (zoneState != null)
                {
                    var state = zoneState.Buildings.Find(b => b.Id == buildingId);
                    Debug.Log($"Found building state? {state != null}");
                    
                    if (state != null)
                    {
                        // Restore building active state
                        isActive = state.IsActive;
                        Debug.Log($"Restored building active state: {isActive}");
                        
                        // Restore staked characters
                        stakedCharacters.Clear();
                        foreach (var characterId in state.StakedCharacterIds)
                        {
                            // Get character NFT from marketplace inventory
                            var nft = MarketplaceManager.Instance.GetCharacterNFTById(characterId);
                            if (nft != null)
                            {
                                stakedCharacters.Add(nft);
                                Debug.Log($"Restored staked character: {characterId}");
                            }
                        }
                        
                        Debug.Log($"Restored {stakedCharacters.Count} staked characters");
                        
                        // Update visuals based on restored state
                        UpdateVisuals();
                        
                        // Restart production if building was active and has characters
                        if (isActive && stakedCharacters.Count > 0)
                        {
                            StartProduction();
                        }
                    }
                    else
                    {
                        // No saved state, ensure building starts inactive
                        isActive = false;
                        stakedCharacters.Clear();
                        UpdateVisuals();
                        Debug.Log($"No saved state found for building {buildingId}, starting inactive");
                    }
                }
            }
        }

        private void SaveState()
        {
            if (gameStateManager == null) return;

            var state = new BuildingState
            {
                Id = buildingId,
                IsActive = isActive,
                StakedCharacterIds = new List<string>(),
                Production = new ProductionState
                {
                    StartTimestamp = (long)GetCurrentTimestamp(),
                    EndTimestamp = (long)(GetCurrentTimestamp() + currentProductionTime),
                    AssetValue = baseFortuneAmount
                }
            };

            // Save staked character IDs
            foreach (var character in stakedCharacters)
            {
                state.StakedCharacterIds.Add(character.Id);
            }

            // Update the state in the zones state
            var gameState = gameStateManager.GetCurrentState();
            if (gameState?.ZonesState?.ZonesList != null)
            {
                var zoneState = gameState.ZonesState.ZonesList.Find(z => z.Id == zoneId);
                if (zoneState != null)
                {
                    var building = zoneState.Buildings.Find(b => b.Id == buildingId);
                    if (building != null)
                    {
                        building.IsActive = state.IsActive;
                        building.StakedCharacterIds = state.StakedCharacterIds;
                        building.Production = state.Production;
                    }
                    else
                    {
                        zoneState.Buildings.Add(state);
                    }
                    gameStateManager.ExportGameState();
                }
            }
        }

        private double GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private void Update()
        {
            if (!isActive || !isProducing) return;

            if (currentProductionTime > 0)
            {
                currentProductionTime -= Time.deltaTime;
                if (currentProductionTime <= 0)
                {
                    canCollect = true;
                    isProducing = false;
                    SaveState();
                }
            }
        }

        public void SetActive(bool active)
        {
            isActive = active;
            UpdateVisuals();
            SaveState();

            // Stop production if building is deactivated
            if (!active)
            {
                StopProduction();
            }
        }

        private void UpdateVisuals()
        {
            if (locationHighlight != null)
            {
                locationHighlight.SetHighlightColor(isActive ? activeColor : inactiveColor);
                locationHighlight.SetInteractable(isActive);
            }
        }

        public bool CanAddCharacter()
        {
            return stakedCharacters.Count < 3;
        }

        public void StakeCharacter(CharacterNFT character)
        {
            if (stakedCharacters.Count >= 3)
            {
                Debug.Log("Cannot stake character - maximum number of characters reached");
                return;
            }
            
            stakedCharacters.Add(character);
            UpdateProductionModifiers();
            
            // Activate building when first character is staked
            if (stakedCharacters.Count == 1)
            {
                SetActive(true);
                StartProduction();
            }
            
            SaveState();
        }

        public void UnstakeCharacter(CharacterNFT character)
        {
            if (stakedCharacters.Remove(character))
            {
                Debug.Log($"Character unstaked from building {gameObject.name}");
                UpdateProductionModifiers();
                
                // Deactivate building if no characters are staked
                if (stakedCharacters.Count == 0)
                {
                    SetActive(false);
                    StopProduction();
                }
                
                SaveState();
            }
        }

        private void StartProduction()
        {
            if (!isActive || stakedCharacters.Count == 0)
            {
                return;
            }

            isProducing = true;
            currentProductionTime = baseProductionTime;
            canCollect = false;
            SaveState();
        }

        private void StopProduction()
        {
            isProducing = false;
            currentProductionTime = 0f;
            canCollect = false;
            SaveState();
        }

        private void UpdateProductionModifiers()
        {
            // Calculate production modifiers based on staked characters
            float speedModifier = 1f;
            float amountModifier = 1f;

            foreach (var character in stakedCharacters)
            {
                speedModifier += character.SpeedModifier;
                amountModifier += character.AmountModifier;
            }

            // Apply modifiers to production values
            currentProductionTime = baseProductionTime / speedModifier;
            baseFortuneAmount = Mathf.RoundToInt(baseFortuneAmount * amountModifier);
            SaveState();
        }

        public void CollectAsset()
        {
            if (!canCollect) return;

            // Add fortune dollars to player balance
            PlayerProgress.Instance.AddFortuneDollars(baseFortuneAmount);
            
            // Reset production
            canCollect = false;
            StartProduction();
        }

        public Sprite GetBuildingSprite()
        {
            return buildingSprite;
        }

        public float GetRemainingProductionTime()
        {
            if (!isProducing) return 0f;
            return Mathf.Max(0f, currentProductionTime);
        }

        public void CollectAssets()
        {
            if (!canCollect) return;
            
            // Add fortune dollars to player progress
            PlayerProgress.Instance.AddFortuneDollars(baseFortuneAmount);
            
            // Reset production
            currentProductionTime = 0f;
            isProducing = false;
            canCollect = false;
            
            // Start new production if building is active and has characters
            if (isActive && stakedCharacters.Count > 0)
            {
                StartProduction();
            }
            
            SaveState();
        }

        private void OnEnable()
        {
            InputTouch.OnFingerTap += OnFingerTap;
        }

        private void OnDisable()
        {
            InputTouch.OnFingerTap -= OnFingerTap;
        }

        private void OnFingerTap(TouchFinger finger)
        {
            if (!isActive || CameraInputs.IsOverUI) return;

            // Convert screen position to ray
            Ray ray = Camera.main.ScreenPointToRay(finger.ScreenPosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log($"Building clicked: {BuildingId}");
                    ZoneManager.Instance?.SetActiveBuilding(this);
                }
            }
        }
    }
} 