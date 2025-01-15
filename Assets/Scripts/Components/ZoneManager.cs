using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace SMZero
{
    public class ZoneManager : MonoBehaviour
    {
        [Header("Zone Settings")]
        [SerializeField] private string zoneName = "Vault Avenue";
        [SerializeField] private string zoneId = "vault_avenue";
        [SerializeField] private Building[] buildings;

        private static ZoneManager instance;
        public static ZoneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ZoneManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("ZoneManager");
                        instance = go.AddComponent<ZoneManager>();
                    }
                }
                return instance;
            }
        }

        public string ZoneName => zoneName;
        public string ZoneId => zoneId;

        private ZoneUI zoneUI;
        private BuildingPopup buildingPopup;
        private Building activeBuilding;
        private ZoneNFT activeZoneNFT;
        private GameStateManager gameStateManager;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            
            // Get references
            zoneUI = FindObjectOfType<ZoneUI>();
            if (zoneUI == null)
            {
                Debug.LogError("ZoneUI not found!");
                return;
            }
            
            // Get BuildingPopup reference from ZoneUI
            buildingPopup = zoneUI.GetBuildingPopup();
            if (buildingPopup == null)
            {
                Debug.LogError("BuildingPopup reference not found in ZoneUI!");
            }
            else
            {
                Debug.Log("BuildingPopup reference obtained from ZoneUI successfully");
            }
            
            // Delay initialization to ensure GameStateManager is ready
            Invoke(nameof(LoadZoneState), 0.2f);
        }

        private void Start()
        {
            // Remove the duplicate initialization in Start since we're doing it in LoadZoneState
            if (zoneUI == null)
            {
                Debug.LogError("ZoneUI not found!");
                return;
            }
        }

        private void LoadZoneState()
        {
            Debug.Log("=== Loading Zone State ===");
            var gameStateManager = GameStateManager.Instance;
            Debug.Log($"Game state manager null? {gameStateManager == null}");
            
            var gameState = gameStateManager?.GetCurrentState();
            Debug.Log($"Game state null? {gameState == null}");
            
            if (gameState == null)
            {
                Debug.LogWarning("No game state found, waiting for initialization...");
                Invoke(nameof(LoadZoneState), 0.2f);
                return;
            }

            // Check if player owns vault-avenue-001
            var marketplaceManager = MarketplaceManager.Instance;
            if (marketplaceManager != null)
            {
                var ownedZones = marketplaceManager.GetOwnedZoneNFTs();
                Debug.Log($"Found {ownedZones?.Count ?? 0} owned zones");
                
                if (ownedZones != null && ownedZones.Count > 0)
                {
                    var vaultAvenueNFT = ownedZones.Find(z => z.Id == "vault-avenue-001");
                    if (vaultAvenueNFT != null)
                    {
                        Debug.Log("Found Vault Avenue NFT");
                        // Set references but don't stake or activate anything
                        if (buildings != null && buildings.Length > 0)
                        {
                            activeBuilding = buildings[0];
                            
                            // Get or create zone state
                            var zoneState = gameState.ZonesState?.ZonesList?.FirstOrDefault(z => z.Id == zoneId);
                            if (zoneState == null)
                            {
                                Debug.Log("Creating new zone state");
                                zoneState = new ZoneStateEntry
                                {
                                    Id = zoneId,
                                    DisplayName = zoneName,
                                    IsActive = false,
                                    Buildings = new List<BuildingState>()
                                };
                                gameState.ZonesState.ZonesList.Add(zoneState);
                            }
                            
                            // Restore zone staking status
                            Debug.Log($"Zone state found - IsActive: {zoneState.IsActive}");
                            if (zoneState.IsActive)
                            {
                                activeZoneNFT = vaultAvenueNFT;
                                Debug.Log("Restored zone NFT as staked");
                            }
                            else
                            {
                                activeZoneNFT = null;
                                Debug.Log("Zone NFT is not staked");
                            }
                            
                            gameStateManager.ExportGameState();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Vault Avenue NFT not found in owned zones!");
                    }
                }
            }
            
            // Initialize UI
            if (zoneUI != null)
            {
                zoneUI.InitializeSlots();
            }
            else
            {
                Debug.LogError("ZoneUI not found during LoadZoneState!");
            }
        }

        public void SaveZoneState()
        {
            Debug.Log("=== Saving Zone State ===");
            var gameStateManager = GameStateManager.Instance;
            if (gameStateManager == null)
            {
                Debug.LogError("GameStateManager not found!");
                return;
            }
            
            var gameState = gameStateManager.GetCurrentState();
            if (gameState?.ZonesState?.ZonesList != null)
            {
                var zoneState = gameState.ZonesState.ZonesList.Find(z => z.Id == zoneId);
                if (zoneState == null)
                {
                    Debug.Log("Creating new zone state entry");
                    zoneState = new ZoneStateEntry
                    {
                        Id = zoneId,
                        DisplayName = zoneName,
                        IsActive = activeZoneNFT != null,
                        Buildings = new List<BuildingState>()
                    };
                    gameState.ZonesState.ZonesList.Add(zoneState);
                }
                else
                {
                    // Update zone staking status
                    zoneState.IsActive = activeZoneNFT != null;
                    Debug.Log($"Updated zone state - IsActive: {zoneState.IsActive}");
                }
                
                // Update building states
                foreach (var building in buildings)
                {
                    if (building != null)
                    {
                        var buildingState = zoneState.Buildings.Find(b => b.Id == building.BuildingId);
                        if (buildingState == null)
                        {
                            Debug.Log($"Creating new building state for {building.BuildingId}");
                            buildingState = new BuildingState
                            {
                                Id = building.BuildingId,
                                IsActive = building.IsActive,
                                StakedCharacterIds = new List<string>(building.GetStakedCharacterIds())
                            };
                            zoneState.Buildings.Add(buildingState);
                        }
                        else
                        {
                            // Update building status and staked characters
                            buildingState.IsActive = building.IsActive;
                            buildingState.StakedCharacterIds.Clear();
                            buildingState.StakedCharacterIds.AddRange(building.GetStakedCharacterIds());
                            Debug.Log($"Updated building state - ID: {building.BuildingId}, IsActive: {building.IsActive}, Staked Characters: {string.Join(", ", buildingState.StakedCharacterIds)}");
                        }
                    }
                }
                
                gameStateManager.ExportGameState();
                Debug.Log("Zone state saved successfully");
            }
            else
            {
                Debug.LogError("Failed to save zone state - game state or zones list is null");
            }
        }

        public Building[] GetBuildings()
        {
            return buildings;
        }

        public void StakeZoneNFT(ZoneNFT zoneNFT)
        {
            Debug.Log($"Staking zone NFT: {zoneNFT?.Id ?? "null"}");
            
            if (activeBuilding == null)
            {
                Debug.LogError("No active building to stake Zone NFT to!");
                return;
            }

            // Update state
            activeZoneNFT = zoneNFT;
            var gameState = GameStateManager.Instance?.GetCurrentState();
            if (gameState?.ZonesState?.ZonesList != null)
            {
                var zoneState = gameState.ZonesState.ZonesList.Find(z => z.Id == zoneId);
                if (zoneState != null)
                {
                    zoneState.IsActive = true;
                    GameStateManager.Instance.ExportGameState();
                    Debug.Log($"Zone state updated - IsActive: {zoneState.IsActive}");
                }
            }
            
            // Set building to active so it can be clicked, but production won't start until a character is staked
            activeBuilding.SetActive(true);
            SaveZoneState();
            
            if (zoneUI != null)
            {
                zoneUI.InitializeSlots();
            }
            else
            {
                Debug.LogError("ZoneUI not found when staking zone NFT!");
            }
            
            Debug.Log($"Zone NFT {zoneNFT?.Id ?? "null"} staked successfully");
        }

        public void UnstakeZoneNFT()
        {
            if (activeBuilding == null || activeZoneNFT == null)
            {
                Debug.LogWarning("No active building or Zone NFT to unstake!");
                return;
            }

            activeBuilding.SetActive(false);
            activeZoneNFT = null;
            SaveZoneState();
            
            // Update UI after unstaking
            if (zoneUI != null)
            {
                zoneUI.InitializeSlots();
            }
        }

        public void SetActiveBuilding(Building building)
        {
            Debug.Log($"Setting active building: {building?.BuildingId ?? "null"}");
            activeBuilding = building;
            
            if (buildingPopup == null)
            {
                Debug.LogError("Cannot show building popup: BuildingPopup reference is null!");
                // Try getting reference from ZoneUI again
                buildingPopup = zoneUI?.GetBuildingPopup();
                if (buildingPopup == null)
                {
                    Debug.LogError("Still cannot get BuildingPopup reference from ZoneUI!");
                    return;
                }
            }
            
            buildingPopup.Show(building);
        }

        public Building GetActiveBuilding()
        {
            return activeBuilding;
        }

        public ZoneNFT GetActiveZoneNFT()
        {
            return activeZoneNFT;
        }
    }
} 