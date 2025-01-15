using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace SMZero
{
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager instance;
        public const string LOCAL_SAVE_KEY = "SMZeroGameState";
        
        public static GameStateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameManagers.Instance.GameState;
                }
                return instance;
            }
        }

        private GameState currentState;
        private static GameState persistentState;

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SaveToLocal(string key, string data);

        [DllImport("__Internal")]
        private static extern string LoadFromLocal(string key);
        #endif

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            Invoke(nameof(InitializeState), 0.1f);
        }

        private void SaveState(string key, string data)
        {
            try
            {
                #if UNITY_WEBGL && !UNITY_EDITOR
                SaveToLocal(key, data);
                #else
                PlayerPrefs.SetString(key, data);
                PlayerPrefs.Save();
                #endif
                DebugOverlay.Instance?.Log("Game state saved successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to save state: {e.Message}");
                DebugOverlay.Instance?.Log($"Failed to save state: {e.Message}");
            }
        }

        private string LoadState(string key)
        {
            try
            {
                #if UNITY_WEBGL && !UNITY_EDITOR
                return LoadFromLocal(key);
                #else
                return PlayerPrefs.GetString(key);
                #endif
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to load state: {e.Message}");
                DebugOverlay.Instance?.Log($"Failed to load state: {e.Message}");
                return null;
            }
        }

        private void InitializeState()
        {
            bool loadedState = false;
            try
            {
                string stateData = LoadState(LOCAL_SAVE_KEY);
                
                // If we have state data, deserialize it
                if (!string.IsNullOrEmpty(stateData))
                {
                    try
                    {
                        currentState = JsonConvert.DeserializeObject<GameState>(stateData);
                        loadedState = true;
                        Debug.Log("Successfully loaded saved game state");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"Failed to deserialize state data: {e.Message}");
                        DebugOverlay.Instance?.Log($"Failed to deserialize state data: {e.Message}");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to load saved state: {e.Message}");
                DebugOverlay.Instance?.Log($"Failed to load saved state: {e.Message}");
            }

            // If no saved state was loaded, initialize with defaults
            if (!loadedState)
            {
                Debug.Log("No saved state found, initializing with defaults");
                currentState = new GameState
                {
                    PlayerBalance = 1000f,
                    PlayerInventory = new PlayerInventory
                    {
                        OwnedCharacterIds = new List<string>(),
                        OwnedZoneIds = new List<string>()
                    },
                    ZonesState = new ZonesState
                    {
                        ZonesList = new List<ZoneStateEntry>()
                    }
                };
            }

            // Initialize player progress with current state's balance
            var playerProgress = PlayerProgress.Instance;
            if (playerProgress != null)
            {
                Debug.Log($"Setting player balance to {currentState.PlayerBalance}");
                playerProgress.SetFortuneDollars(currentState.PlayerBalance);
            }
            else
            {
                Debug.LogWarning("Failed to get PlayerProgress instance!");
            }

            // Initialize marketplace with current state's inventory
            if (MarketplaceManager.Instance != null)
            {
                var marketplaceState = new MarketplaceState
                {
                    PlayerInventory = currentState.PlayerInventory,
                    PlayerBalance = currentState.PlayerBalance
                };
                MarketplaceManager.Instance.RestoreState(marketplaceState);
            }

            Debug.Log($"Game state initialized:");
            Debug.Log($"- Balance: {currentState.PlayerBalance}");
            Debug.Log($"- Owned Characters: {currentState.PlayerInventory?.OwnedCharacterIds?.Count ?? 0}");
            Debug.Log($"- Owned Zones: {currentState.PlayerInventory?.OwnedZoneIds?.Count ?? 0}");
            
            ExportGameState();
            Debug.Log("=== Game State Initialization Complete ===");
        }

        public void ExportGameState()
        {
            try
            {
                DebugOverlay.Instance?.Log("=== Exporting Game State ===");
                
                // Sync current state with latest data
                if (currentState != null)
                {
                    // Update balance
                    if (PlayerProgress.Instance != null)
                    {
                        currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();
                        DebugOverlay.Instance?.Log($"Updated balance to: {currentState.PlayerBalance}");
                    }
                    
                    // Update inventory from MarketplaceManager
                    if (MarketplaceManager.Instance != null)
                    {
                        var marketplaceState = MarketplaceManager.Instance.GetCurrentState();
                        if (marketplaceState?.PlayerInventory != null)
                        {
                            currentState.PlayerInventory = marketplaceState.PlayerInventory;
                            DebugOverlay.Instance?.Log($"Updated inventory from MarketplaceManager");
                        }
                    }
                }

                string stateData = JsonConvert.SerializeObject(currentState, new JsonSerializerSettings 
                { 
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                
                SaveState(LOCAL_SAVE_KEY, stateData);
                persistentState = currentState;
                LogGameState("Game state exported successfully", currentState);
                DebugOverlay.Instance?.Log("=== Export Complete ===");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to export game state: {e.Message}\n{e.StackTrace}");
                DebugOverlay.Instance?.Log($"Failed to export game state: {e.Message}");
            }
        }

        private void LogGameState(string prefix, GameState state)
        {
            if (state == null)
            {
                DebugOverlay.Instance.Log($"{prefix} - State is null!");
                return;
            }

            DebugOverlay.Instance.Log($"{prefix}:");
            DebugOverlay.Instance.Log($"- Balance: {state.PlayerBalance}");
            
            if (state.PlayerInventory != null)
            {
                DebugOverlay.Instance.Log($"- Inventory:");
                DebugOverlay.Instance.Log($"  - Owned Characters: {state.PlayerInventory.OwnedCharacterIds?.Count ?? 0}");
                foreach (var id in state.PlayerInventory.OwnedCharacterIds ?? new List<string>())
                {
                    DebugOverlay.Instance.Log($"    - Character: {id}");
                }
                DebugOverlay.Instance.Log($"  - Owned Zones: {state.PlayerInventory.OwnedZoneIds?.Count ?? 0}");
                foreach (var id in state.PlayerInventory.OwnedZoneIds ?? new List<string>())
                {
                    DebugOverlay.Instance.Log($"    - Zone: {id}");
                }
            }
            
            if (state.ZonesState?.ZonesList != null)
            {
                DebugOverlay.Instance.Log("- Zones State:");
                DebugOverlay.Instance.Log($"  - Total Zones: {state.ZonesState.ZonesList.Count}");
            }
        }

        public GameState GetCurrentState()
        {
            if (currentState != null && PlayerProgress.Instance != null)
            {
                currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();
            }
            return currentState;
        }

        public void ResetState()
        {
            currentState = new GameState();
            persistentState = null;
            
            // Clear marketplace state
            if (MarketplaceManager.Instance != null)
            {
                var marketplaceState = new MarketplaceState();
                marketplaceState.PlayerInventory = new PlayerInventory();
                MarketplaceManager.Instance.RestoreState(marketplaceState);
            }
            
            ExportGameState();
        }
    }

    [System.Serializable]
    public class GameState
    {
        [JsonProperty("b")]
        public float PlayerBalance { get; set; }
        
        [JsonProperty("i")]
        public PlayerInventory PlayerInventory { get; set; }
        
        [JsonProperty("z")]
        public ZonesState ZonesState { get; set; }

        public GameState()
        {
            PlayerBalance = 1000f;
            PlayerInventory = new PlayerInventory();
            ZonesState = new ZonesState();
        }
    }

    [System.Serializable]
    public class ZonesState
    {
        [JsonProperty("l")]
        public List<ZoneStateEntry> ZonesList { get; set; } = new List<ZoneStateEntry>();
    }

    [System.Serializable]
    public class ZoneStateEntry
    {
        [JsonProperty("i")]
        public string Id { get; set; }
        
        [JsonProperty("n")]
        public string DisplayName { get; set; }
        
        [JsonProperty("a")]
        public bool IsActive { get; set; }
        
        [JsonProperty("b")]
        public List<BuildingState> Buildings { get; set; } = new List<BuildingState>();
    }

    [System.Serializable]
    public class BuildingState
    {
        [JsonProperty("i")]
        public string Id { get; set; }
        
        [JsonProperty("a")]
        public bool IsActive { get; set; }
        
        [JsonProperty("c")]
        public List<string> StakedCharacterIds { get; set; } = new List<string>();
        
        [JsonProperty("p")]
        public ProductionState Production { get; set; }
    }

    [System.Serializable]
    public class ProductionState
    {
        [JsonProperty("s")]
        public long StartTimestamp { get; set; }
        
        [JsonProperty("e")]
        public long EndTimestamp { get; set; }
        
        [JsonProperty("v")]
        public float AssetValue { get; set; }
    }
} 