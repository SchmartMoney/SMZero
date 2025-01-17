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
        private bool isInitialized = false;
        public bool IsInitialized => isInitialized;

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SaveToLocal(string key, string data);

        [DllImport("__Internal")]
        private static extern string LoadFromLocal(string key);
        #endif

        private void Awake()
        {
            UnityEngine.Debug.Log("[GameStateManager] Awake called");
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            InitializeState();
        }

        private void SaveState(string key, string data)
        {
            try
            {
                UnityEngine.Debug.Log($"[GameStateManager] Saving state data (length: {data?.Length ?? 0})");
                UnityEngine.Debug.Log($"[GameStateManager] Full state data being saved: {data}");
                #if UNITY_WEBGL && !UNITY_EDITOR
                SaveToLocal(key, data);
                UnityEngine.Debug.Log("[GameStateManager] Saved state to WebGL localStorage");
                #else
                PlayerPrefs.SetString(key, data);
                PlayerPrefs.Save();
                UnityEngine.Debug.Log("[GameStateManager] Saved state to PlayerPrefs");
                #endif
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[GameStateManager] Failed to save state: {e.Message}");
            }
        }

        private string LoadState(string key)
        {
            try
            {
                string data = null;
                #if UNITY_WEBGL && !UNITY_EDITOR
                data = LoadFromLocal(key);
                UnityEngine.Debug.Log("[GameStateManager] Loaded state from WebGL localStorage");
                #else
                data = PlayerPrefs.GetString(key);
                UnityEngine.Debug.Log("[GameStateManager] Loaded state from PlayerPrefs");
                #endif
                UnityEngine.Debug.Log($"[GameStateManager] Loaded state data (length: {data?.Length ?? 0})");
                UnityEngine.Debug.Log($"[GameStateManager] Full state data loaded: {data}");
                return data;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning($"[GameStateManager] Failed to load state: {e.Message}");
                return null;
            }
        }

        private void LogGameState(string prefix, GameState state)
        {
            if (state == null)
            {
                UnityEngine.Debug.Log($"{prefix} - State is null!");
                return;
            }

            UnityEngine.Debug.Log($"{prefix}:");
            UnityEngine.Debug.Log($"- Balance: {state.PlayerBalance}");
            
            if (state.PlayerInventory != null)
            {
                UnityEngine.Debug.Log($"- Inventory:");
                UnityEngine.Debug.Log($"  - Owned Characters ({state.PlayerInventory.OwnedCharacterIds?.Count ?? 0}):");
                if (state.PlayerInventory.OwnedCharacterIds != null)
                {
                    foreach (var id in state.PlayerInventory.OwnedCharacterIds)
                    {
                        UnityEngine.Debug.Log($"    - {id}");
                    }
                }
                UnityEngine.Debug.Log($"  - Owned Zones ({state.PlayerInventory.OwnedZoneIds?.Count ?? 0}):");
                if (state.PlayerInventory.OwnedZoneIds != null)
                {
                    foreach (var id in state.PlayerInventory.OwnedZoneIds)
                    {
                        UnityEngine.Debug.Log($"    - {id}");
                    }
                }
            }
            else
            {
                UnityEngine.Debug.Log("- Inventory is null!");
            }
        }

        private void InitializeState()
        {
            UnityEngine.Debug.Log("[GameStateManager] Starting state initialization");
            bool loadedState = false;
            try
            {
                string stateData = LoadState(LOCAL_SAVE_KEY);
                
                if (!string.IsNullOrEmpty(stateData))
                {
                    try
                    {
                        currentState = JsonConvert.DeserializeObject<GameState>(stateData);
                        loadedState = true;
                        LogGameState("[GameStateManager] Successfully loaded state", currentState);
                        
                        // Ensure lists are initialized
                        if (currentState.PlayerInventory == null)
                        {
                            currentState.PlayerInventory = new PlayerInventory
                            {
                                OwnedCharacterIds = new List<string>(),
                                OwnedZoneIds = new List<string>()
                            };
                        }
                        else
                        {
                            currentState.PlayerInventory.OwnedCharacterIds ??= new List<string>();
                            currentState.PlayerInventory.OwnedZoneIds ??= new List<string>();
                        }
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.LogError($"[GameStateManager] Failed to deserialize state data: {e.Message}");
                    }
                }
                else
                {
                    UnityEngine.Debug.Log("[GameStateManager] No saved state found, creating new state");
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[GameStateManager] Failed to load saved state: {e.Message}");
            }

            if (!loadedState)
            {
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
                UnityEngine.Debug.Log("[GameStateManager] Created new default state");
                LogGameState("[GameStateManager] New state details", currentState);
                ExportGameState(); // Save the initial state
            }

            var playerProgress = PlayerProgress.Instance;
            if (playerProgress != null)
            {
                playerProgress.SetFortuneDollars(currentState.PlayerBalance);
                UnityEngine.Debug.Log($"[GameStateManager] Set player balance to {currentState.PlayerBalance}");
            }
            else
            {
                UnityEngine.Debug.LogError("[GameStateManager] Failed to get PlayerProgress instance!");
            }
            
            isInitialized = true;
            UnityEngine.Debug.Log("[GameStateManager] State initialization complete");
        }

        public void ExportGameState()
        {
            try
            {
                UnityEngine.Debug.Log("[GameStateManager] Starting game state export");
                LogGameState("[GameStateManager] Current state before export", currentState);
                
                // Sync current state with latest data
                if (currentState != null)
                {
                    // Update balance
                    if (PlayerProgress.Instance != null)
                    {
                        currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();
                        UnityEngine.Debug.Log($"[GameStateManager] Updated balance to: {currentState.PlayerBalance}");
                    }
                    
                    // Update inventory from MarketplaceManager
                    if (MarketplaceManager.Instance != null)
                    {
                        var marketplaceState = MarketplaceManager.Instance.GetCurrentState();
                        if (marketplaceState?.PlayerInventory != null)
                        {
                            currentState.PlayerInventory = marketplaceState.PlayerInventory;
                            UnityEngine.Debug.Log($"[GameStateManager] Updated inventory from MarketplaceManager");
                            LogGameState("[GameStateManager] Updated state after marketplace sync", currentState);
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
                UnityEngine.Debug.Log("[GameStateManager] Game state exported successfully");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[GameStateManager] Failed to export game state: {e.Message}\n{e.StackTrace}");
            }
        }

        public GameState GetCurrentState()
        {
            if (!isInitialized)
            {
                UnityEngine.Debug.LogWarning("[GameStateManager] Attempting to get state before initialization");
                return null;
            }

            if (currentState != null && PlayerProgress.Instance != null)
            {
                currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();
            }
            return currentState;
        }

        public void ResetState()
        {
            UnityEngine.Debug.Log("[GameStateManager] Resetting game state");
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
            UnityEngine.Debug.Log("[GameStateManager] Game state reset complete");
        }

        private void OnApplicationQuit()
        {
            UnityEngine.Debug.Log("[GameStateManager] Application quitting, exporting final state");
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