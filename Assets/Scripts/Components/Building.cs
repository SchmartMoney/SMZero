using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Exoa.Touch;
using Exoa.Events;
using Exoa.Cameras;

namespace SMZero
{
    public class Building : MonoBehaviour
    {
        private const float BASE_PRODUCTION_TIME = 3600f; // 1 hour in seconds
        private const float BASE_FORTUNE_AMOUNT = 50f;
        
        [SerializeField] private string buildingId;
        [SerializeField] private string zoneId;
        [SerializeField] private Sprite buildingSprite;
        
        private LocationHighlight locationHighlight;
        private GameStateManager gameStateManager;
        private List<CharacterNFT> stakedCharacters = new List<CharacterNFT>();
        
        private bool isActive;
        private bool isProducing;
        private bool canCollect;
        private float currentProductionTime;
        private float baseProductionTime = BASE_PRODUCTION_TIME;
        private int baseFortuneAmount = (int)BASE_FORTUNE_AMOUNT;
        private float productionProgress;
        private float lastUpdateTime;

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
        public float ProductionProgress => productionProgress;

        private void Awake()
        {
            locationHighlight = GetComponent<LocationHighlight>();
            gameStateManager = GameStateManager.Instance;
            lastUpdateTime = Time.time;
            
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

            // Load state after a short delay to ensure all managers are initialized
            Invoke(nameof(LoadState), 0.2f);
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
                    var buildingState = zoneState.Buildings.Find(b => b.Id == buildingId);
                    if (buildingState != null)
                    {
                        Debug.Log($"Loading state for building {buildingId}");
                        
                        // Restore active state
                        isActive = buildingState.IsActive;
                        if (locationHighlight != null)
                        {
                            locationHighlight.SetHighlightColor(isActive ? Color.yellow : Color.gray);
                            locationHighlight.SetInteractable(isActive);
                        }

                        // Restore staked characters
                        stakedCharacters.Clear();
                        var marketplaceManager = MarketplaceManager.Instance;
                        if (marketplaceManager != null)
                        {
                            foreach (var characterId in buildingState.StakedCharacterIds)
                            {
                                var character = marketplaceManager.GetCharacterNFTById(characterId);
                                if (character != null)
                                {
                                    stakedCharacters.Add(character);
                                    character.InitializeRuntime();
                                }
                            }
                        }

                        // Restore production state if active and has characters
                        if (buildingState.Production != null && isActive && stakedCharacters.Count > 0)
                        {
                            UpdateProductionModifiers();
                            
                            // Calculate remaining production time
                            long currentTime = (long)GetCurrentTimestamp();
                            long startTime = buildingState.Production.StartTimestamp;
                            long endTime = buildingState.Production.EndTimestamp;
                            
                            if (currentTime >= endTime)
                            {
                                // Production is complete
                                isProducing = true;
                                canCollect = true;
                                productionProgress = currentProductionTime;
                            }
                            else if (currentTime > startTime)
                            {
                                // Production is in progress
                                isProducing = true;
                                canCollect = false;
                                productionProgress = (currentTime - startTime);
                            }
                            else
                            {
                                // Start new production
                                StartProduction();
                            }
                        }
                        
                        Debug.Log($"Building state loaded - Active: {isActive}, Characters: {stakedCharacters.Count}, Producing: {isProducing}");
                    }
                }
            }
        }

        private void Update()
        {
            if (!isProducing || !isActive) return;

            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;

            // Update production progress
            if (!canCollect)
            {
                productionProgress += deltaTime;
                if (productionProgress >= currentProductionTime)
                {
                    canCollect = true;
                    productionProgress = currentProductionTime;
                }
            }

            // Update character passive income
            foreach (var character in stakedCharacters)
            {
                character.accumulatedIncome += character.progressRate * deltaTime;
                if (character.accumulatedIncome >= 1f) // Collect when at least 1 FD accumulated
                {
                    int amount = Mathf.FloorToInt(character.accumulatedIncome);
                    PlayerProgress.Instance.AddFortuneDollars(amount);
                    character.accumulatedIncome -= amount;
                }
            }
        }

        private void UpdateProductionModifiers()
        {
            float speedModifier = 1f;
            float amountModifier = 1f;

            foreach (var character in stakedCharacters)
            {
                // Emily reduces production time by 15%
                if (character.Name == "Emily")
                {
                    speedModifier = 0.85f;
                }
                // Jake doubles asset value
                if (character.Name == "Jake")
                {
                    amountModifier = 2f;
                }
            }

            // Apply modifiers
            currentProductionTime = BASE_PRODUCTION_TIME * speedModifier;
            baseFortuneAmount = Mathf.RoundToInt(BASE_FORTUNE_AMOUNT * amountModifier);
            
            Debug.Log($"Production modifiers updated - Speed: {speedModifier}, Amount: {amountModifier}");
            Debug.Log($"New production values - Time: {currentProductionTime}s, Amount: {baseFortuneAmount}FD");
        }

        public void StakeCharacter(CharacterNFT character)
        {
            if (stakedCharacters.Count >= 3)
            {
                Debug.Log("Cannot stake character - maximum number of characters reached");
                return;
            }
            
            stakedCharacters.Add(character);
            character.InitializeRuntime(); // Reset accumulated income
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
            productionProgress = 0f;
            canCollect = false;
            SaveState();
        }

        private void StopProduction()
        {
            isProducing = false;
            productionProgress = 0f;
            canCollect = false;
            SaveState();
        }

        public void CollectAsset()
        {
            if (!canCollect) return;

            // Add fortune dollars to player balance
            PlayerProgress.Instance.AddFortuneDollars(baseFortuneAmount);
            
            // Reset production
            canCollect = false;
            productionProgress = 0f;
            StartProduction();
        }

        public float GetRemainingProductionTime()
        {
            if (!isProducing || canCollect) return 0f;
            return currentProductionTime - productionProgress;
        }


        public void SpeedUpProduction()
        {
            if (!isProducing || canCollect) return;
            productionProgress = currentProductionTime - 5f;
            SaveState();
        }

        public void SetActive(bool active)
        {
            isActive = active;
            if (locationHighlight != null)
            {
                locationHighlight.SetHighlightColor(active ? Color.yellow : Color.gray);
                locationHighlight.SetInteractable(active);
            }
            SaveState();

            // Stop production if building is deactivated
            if (!active)
            {
                StopProduction();
            }
        }

        private void SaveState()
        {
            if (gameStateManager == null) return;

            var state = new BuildingState
            {
                Id = buildingId,
                IsActive = isActive,
                StakedCharacterIds = stakedCharacters.Select(c => c.Id).ToList(),
                Production = new ProductionState
                {
                    StartTimestamp = (long)GetCurrentTimestamp(),
                    EndTimestamp = (long)(GetCurrentTimestamp() + (currentProductionTime - productionProgress)),
                    AssetValue = baseFortuneAmount
                }
            };

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

        public List<string> GetStakedCharacterIds()
        {
            return stakedCharacters.Select(c => c.Id).ToList();
        }

        public Sprite GetBuildingSprite()
        {
            return buildingSprite;
        }

        public void CollectAssets()
        {
            // This is just an alias for CollectAsset to maintain backward compatibility
            CollectAsset();
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

        // ... rest of the existing code ...
    }
} 