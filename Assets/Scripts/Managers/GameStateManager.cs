using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SMZero
{
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager instance;
        public const string LOCAL_SAVE_KEY = "SMZeroGameState";
        private const string CLOUD_SAVE_KEY = "SMZeroGameState";
        
        public static GameStateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameStateManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameStateManager");
                        instance = go.AddComponent<GameStateManager>();
                    }
                }
                return instance;
            }
        }

        private GameState currentState;
        private static GameState persistentState;

        // JavaScript interface for Telegram Web App
        [DllImport("__Internal")]
        private static extern void SaveToCloud(string key, string data);

        [DllImport("__Internal")]
        private static extern string LoadFromCloud(string key);

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Delay initialization to ensure other managers are ready
            Invoke(nameof(InitializeState), 0.1f);
        }

        private void SaveToLocal(string stateData)
        {
            PlayerPrefs.SetString(LOCAL_SAVE_KEY, stateData);
            PlayerPrefs.Save();
            DebugOverlay.Instance.Log("State saved to local storage");
        }

        private string LoadFromLocal()
        {
            return PlayerPrefs.GetString(LOCAL_SAVE_KEY);
        }

        private void InitializeState()
        {
            Debug.Log("=== Initializing Game State ===");
            
            // Try to load saved state first
            bool loadedState = false;
            try
            {
                // Try loading from cloud first
                string stateData = null;
                try
                {
                    stateData = LoadFromCloud(CLOUD_SAVE_KEY);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to load from cloud: {e.Message}");
                }

                // If cloud load failed, try local storage
                if (string.IsNullOrEmpty(stateData))
                {
                    stateData = LoadFromLocal();
                }

                // If we have state data, deserialize it
                if (!string.IsNullOrEmpty(stateData))
                {
                    var serializer = new XmlSerializer(typeof(GameState));
                    using (var reader = new StringReader(stateData))
                    {
                        currentState = (GameState)serializer.Deserialize(reader);
                        loadedState = true;
                        Debug.Log("Successfully loaded saved game state");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load saved state: {e.Message}");
            }

            // If no saved state was loaded, initialize with defaults
            if (!loadedState)
            {
                Debug.Log("No saved state found, initializing with defaults");
                currentState = new GameState
                {
                    PlayerBalance = 1000f, // Default starting balance
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
                Debug.LogError("Failed to get PlayerProgress instance!");
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
                DebugOverlay.Instance.Log("=== Exporting Game State ===");
                
                // Sync current state with latest data
                if (currentState != null)
                {
                    // Update balance
                    if (PlayerProgress.Instance != null)
                    {
                        currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();
                        DebugOverlay.Instance.Log($"Updated balance to: {currentState.PlayerBalance}");
                    }
                    
                    // Update inventory from MarketplaceManager
                    if (MarketplaceManager.Instance != null)
                    {
                        var marketplaceState = MarketplaceManager.Instance.GetCurrentState();
                        if (marketplaceState?.PlayerInventory != null)
                        {
                            currentState.PlayerInventory = marketplaceState.PlayerInventory;
                            DebugOverlay.Instance.Log($"Updated inventory from MarketplaceManager");
                        }
                    }
                }

                var serializer = new XmlSerializer(typeof(GameState));
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, currentState);
                    string stateData = writer.ToString();
                    
                    DebugOverlay.Instance.Log($"State to save: {stateData}");
                    
                    // Try to save to cloud first
                    bool cloudSaveFailed = false;
                    try
                    {
                        SaveToCloud(CLOUD_SAVE_KEY, stateData);
                        DebugOverlay.Instance.Log("Game state exported successfully to cloud");
                    }
                    catch (System.Exception e)
                    {
                        DebugOverlay.Instance.Log($"Failed to save to cloud: {e.Message}\nFalling back to local storage...");
                        cloudSaveFailed = true;
                    }

                    // If cloud save failed, save locally
                    if (cloudSaveFailed)
                    {
                        SaveToLocal(stateData);
                    }

                    persistentState = currentState;
                    LogGameState("Game state exported successfully", currentState);
                }
                DebugOverlay.Instance.Log("=== Export Complete ===");
            }
            catch (System.Exception e)
            {
                DebugOverlay.Instance.Log($"Failed to export game state: {e.Message}\n{e.StackTrace}");
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
        public float PlayerBalance { get; set; }
        public PlayerInventory PlayerInventory { get; set; }
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
        public List<ZoneStateEntry> ZonesList { get; set; } = new List<ZoneStateEntry>();
    }

    [System.Serializable]
    public class ZoneStateEntry
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public List<BuildingState> Buildings { get; set; } = new List<BuildingState>();
    }

    [System.Serializable]
    public class BuildingState
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public List<string> StakedCharacterIds { get; set; } = new List<string>();
        public ProductionState Production { get; set; }
    }

    [System.Serializable]
    public class ProductionState
    {
        public long StartTimestamp { get; set; }
        public long EndTimestamp { get; set; }
        public float AssetValue { get; set; }
    }
} 