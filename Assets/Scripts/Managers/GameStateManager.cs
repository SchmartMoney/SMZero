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
            DebugOverlay.Instance.Log("=== Initializing Game State ===");
            
            bool isNewState = false;
            
            // Try to load state from memory first
            if (persistentState != null)
            {
                DebugOverlay.Instance.Log("Using persistent state from memory");
                currentState = persistentState;
                LogGameState("Loaded persistent state", currentState);
            }
            else
            {
                DebugOverlay.Instance.Log("No persistent state in memory, attempting to load from cloud...");
                string savedState = null;
                bool cloudLoadFailed = false;

                // Try cloud storage first
                try
                {
                    savedState = LoadFromCloud(CLOUD_SAVE_KEY);
                }
                catch (System.Exception e)
                {
                    DebugOverlay.Instance.Log($"Failed to load state from cloud: {e.Message}\nTrying local storage...");
                    cloudLoadFailed = true;
                }

                // If cloud failed or returned empty, try local storage
                if (cloudLoadFailed || string.IsNullOrEmpty(savedState))
                {
                    savedState = LoadFromLocal();
                    if (!string.IsNullOrEmpty(savedState))
                    {
                        DebugOverlay.Instance.Log("Found state in local storage");
                    }
                }

                if (!string.IsNullOrEmpty(savedState))
                {
                    DebugOverlay.Instance.Log($"Raw state loaded: {savedState}");
                    var serializer = new XmlSerializer(typeof(GameState));
                    using (var reader = new StringReader(savedState))
                    {
                        try
                        {
                            currentState = (GameState)serializer.Deserialize(reader);
                            if (currentState != null)
                            {
                                LogGameState("Successfully loaded state", currentState);
                            }
                            else
                            {
                                DebugOverlay.Instance.Log("Deserialized state is null, will create new state");
                                isNewState = true;
                            }
                        }
                        catch (System.InvalidOperationException e)
                        {
                            DebugOverlay.Instance.Log($"Failed to deserialize state: {e.Message}\nWill create new state");
                            currentState = null;
                            isNewState = true;
                        }
                    }
                }
                else
                {
                    DebugOverlay.Instance.Log("No saved state found, will create new state");
                    isNewState = true;
                }

                if (currentState == null)
                {
                    DebugOverlay.Instance.Log("Creating new game state with default values");
                    currentState = new GameState();
                    LogGameState("Created new game state", currentState);
                }
            }

            // Initialize marketplace state
            if (MarketplaceManager.Instance != null)
            {
                DebugOverlay.Instance.Log("Initializing marketplace state...");
                var marketplaceState = new MarketplaceState();
                var inventory = new PlayerInventory();
                
                if (currentState.PlayerInventory != null)
                {
                    var currentInventory = currentState.PlayerInventory;
                    inventory.OwnedCharacterIds = new List<string>(currentInventory.OwnedCharacterIds ?? new List<string>());
                    inventory.OwnedZoneIds = new List<string>(currentInventory.OwnedZoneIds ?? new List<string>());
                    DebugOverlay.Instance.Log($"Restored inventory - Characters: {inventory.OwnedCharacterIds.Count}, Zones: {inventory.OwnedZoneIds.Count}");
                }
                else
                {
                    DebugOverlay.Instance.Log("No existing inventory found, initializing empty inventory");
                }
                
                marketplaceState.PlayerInventory = inventory;
                MarketplaceManager.Instance.RestoreState(marketplaceState);
            }
            else
            {
                DebugOverlay.Instance.Log("Warning: MarketplaceManager instance not found");
            }

            // Update player balance only if this is a new state
            if (PlayerProgress.Instance != null)
            {
                if (isNewState)
                {
                    DebugOverlay.Instance.Log($"New state: Setting initial balance to {currentState.PlayerBalance}");
                    PlayerProgress.Instance.SetFortuneDollars(currentState.PlayerBalance);
                }
                else
                {
                    DebugOverlay.Instance.Log($"Existing state: Loading balance of {currentState.PlayerBalance}");
                    PlayerProgress.Instance.SetFortuneDollars(currentState.PlayerBalance);
                }
            }
            else
            {
                DebugOverlay.Instance.Log("Warning: PlayerProgress instance not found");
            }

            ExportGameState();
            DebugOverlay.Instance.Log("=== State Initialization Complete ===");
            
            // Show debug overlay
            DebugOverlay.Instance.Show();
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