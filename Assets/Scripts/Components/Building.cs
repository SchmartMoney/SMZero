using UnityEngine;
using System.Collections.Generic;
using System;

namespace SMZero
{
    public class Building : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float baseProductionTime = 10f; // 10 seconds for testing
        [SerializeField] private int baseFortuneAmount = 100;
        [SerializeField] private string buildingId = "vault_avenue_main"; // Default to main building
        [SerializeField] private string zoneId = "vault_avenue"; // Default to Vault Avenue zone

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
        public IReadOnlyList<CharacterNFT> StakedCharacters => stakedCharacters.AsReadOnly();
        public int BaseFortuneAmount => baseFortuneAmount;
        public string BuildingId => buildingId;
        public string ZoneId => zoneId;

        private void Awake()
        {
            locationHighlight = GetComponent<LocationHighlight>();
            gameStateManager = GameStateManager.Instance;
            
            if (locationHighlight == null)
            {
                Debug.LogError($"Building {gameObject.name} is missing LocationHighlight component!");
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
            // Main building: "vault_avenue_main"
            // Other buildings: "vault_avenue_building_1" through "vault_avenue_building_9"
            if (buildingId == "vault_avenue_main")
            {
                Debug.Log("This is the main Vault Avenue building");
            }
            else if (buildingId.StartsWith("vault_avenue_building_"))
            {
                string numberPart = buildingId.Replace("vault_avenue_building_", "");
                if (int.TryParse(numberPart, out int buildingNumber))
                {
                    if (buildingNumber >= 1 && buildingNumber <= 9)
                    {
                        Debug.Log($"This is Vault Avenue building #{buildingNumber}");
                    }
                    else
                    {
                        Debug.LogError($"Invalid building number: {buildingNumber}. Should be between 1 and 9.");
                    }
                }
                else
                {
                    Debug.LogError($"Invalid building ID format: {buildingId}");
                }
            }
            else
            {
                Debug.LogError($"Invalid building ID format: {buildingId}. Should be either 'vault_avenue_main' or 'vault_avenue_building_X' where X is 1-9");
            }
        }

        private void LoadState()
        {
            if (gameStateManager == null) return;

            var gameState = gameStateManager.GetCurrentState();
            if (gameState?.ZonesState?.ZonesList != null)
            {
                var zoneState = gameState.ZonesState.ZonesList.Find(z => z.Id == zoneId);
                if (zoneState != null)
                {
                    var state = zoneState.Buildings.Find(b => b.Id == buildingId);
                    if (state != null)
                    {
                        isActive = state.IsActive;
                        
                        // Restore staked characters
                        stakedCharacters.Clear();
                        foreach (var characterId in state.StakedCharacterIds)
                        {
                            // Get character NFT from marketplace inventory
                            var nft = MarketplaceManager.Instance.GetCharacterNFTById(characterId);
                            if (nft != null)
                            {
                                stakedCharacters.Add(nft);
                            }
                        }

                        // Restore production state
                        if (state.Production != null)
                        {
                            var currentTime = (long)GetCurrentTimestamp();
                            isProducing = state.Production.EndTimestamp > currentTime;
                            canCollect = isProducing && state.Production.EndTimestamp <= currentTime;
                            if (isProducing)
                            {
                                currentProductionTime = state.Production.EndTimestamp - currentTime;
                            }
                        }

                        UpdateProductionModifiers();
                        UpdateVisuals();
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
            return isActive && stakedCharacters.Count < 3;
        }

        public void StakeCharacter(CharacterNFT character)
        {
            if (!isActive)
            {
                Debug.Log("Cannot stake character - building not active (needs Zone NFT)");
                return;
            }

            if (stakedCharacters.Count >= 3)
            {
                Debug.Log("Cannot stake character - maximum number of characters reached");
                return;
            }
            
            stakedCharacters.Add(character);
            UpdateProductionModifiers();
            SaveState();
            
            // Start production when first character is staked
            if (!isProducing && stakedCharacters.Count == 1)
            {
                StartProduction();
            }
        }

        public void UnstakeCharacter(CharacterNFT character)
        {
            if (stakedCharacters.Remove(character))
            {
                Debug.Log($"Character unstaked from building {gameObject.name}");
                UpdateProductionModifiers();
                SaveState();

                // Stop production if no characters left
                if (stakedCharacters.Count == 0)
                {
                    StopProduction();
                }
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
    }
} 