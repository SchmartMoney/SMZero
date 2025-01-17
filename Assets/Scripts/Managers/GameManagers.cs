using UnityEngine;
using UnityEngine.SceneManagement;

namespace SMZero
{
    public class GameManagers : MonoBehaviour
    {
        private static GameManagers instance;
        private static bool isQuitting = false;
        private bool isInitialized = false;
        
        public static GameManagers Instance
        {
            get
            {
                if (isQuitting)
                {
                    return null;
                }
                
                if (instance == null)
                {
                    // Try to find existing instance first
                    instance = FindObjectOfType<GameManagers>();
                    
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManagers");
                        instance = go.AddComponent<GameManagers>();
                        DontDestroyOnLoad(go);
                        UnityEngine.Debug.Log("[GameManagers] Created new instance");
                    }
                }
                return instance;
            }
        }

        [SerializeField] private DebugOverlay debugOverlay;
        [SerializeField] private GameHUD gameHUD;
        [SerializeField] private GameStateManager gameState;
        [SerializeField] private PlayerProgress playerProgress;
        [SerializeField] private MarketplaceManager marketplace;

        public DebugOverlay Debug => debugOverlay;
        public GameHUD HUD => gameHUD;
        public GameStateManager GameState => gameState;
        public PlayerProgress Progress => playerProgress;
        public MarketplaceManager Marketplace => marketplace;

        private void Awake()
        {
            UnityEngine.Debug.Log("[GameManagers] Awake called");
            if (instance != null && instance != this)
            {
                UnityEngine.Debug.Log("[GameManagers] Destroying duplicate instance");
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.Debug.Log("[GameManagers] Starting initialization");
            
            // Initialize all managers in the correct order
            InitializeManagers();
            InitializeStateManagers();
            
            // Subscribe to scene load events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Ensure managers are initialized when a new scene loads
            if (instance == this && !isInitialized)
            {
                InitializeManagers();
                InitializeStateManagers();
            }
        }

        private void InitializeManagers()
        {
            UnityEngine.Debug.Log("[GameManagers] Initializing core managers");

            // First, create core managers that don't have dependencies
            if (debugOverlay == null)
            {
                GameObject debugObj = new GameObject("DebugOverlay");
                debugObj.transform.SetParent(transform);
                debugOverlay = debugObj.AddComponent<DebugOverlay>();
                UnityEngine.Debug.Log("[GameManagers] Created DebugOverlay");
            }

            if (playerProgress == null)
            {
                GameObject progressObj = new GameObject("PlayerProgress");
                progressObj.transform.SetParent(transform);
                playerProgress = progressObj.AddComponent<PlayerProgress>();
                UnityEngine.Debug.Log("[GameManagers] Created PlayerProgress");
            }

            if (marketplace == null)
            {
                GameObject marketplaceObj = new GameObject("Marketplace");
                marketplaceObj.transform.SetParent(transform);
                marketplace = marketplaceObj.AddComponent<MarketplaceManager>();
                UnityEngine.Debug.Log("[GameManagers] Created MarketplaceManager");
            }

            UnityEngine.Debug.Log("[GameManagers] Core managers initialized");
        }

        private void InitializeStateManagers()
        {
            UnityEngine.Debug.Log("[GameManagers] Initializing state-dependent managers");

            // Then create managers that depend on PlayerProgress
            if (gameState == null)
            {
                GameObject stateObj = new GameObject("GameState");
                stateObj.transform.SetParent(transform);
                gameState = stateObj.AddComponent<GameStateManager>();
                UnityEngine.Debug.Log("[GameManagers] Created GameStateManager");
            }

            // Finally create UI managers that depend on game state
            if (gameHUD == null)
            {
                // Load GameHUD prefab
                GameObject hudPrefab = Resources.Load<GameObject>("Prefabs/UI/GameHUD");
                if (hudPrefab != null)
                {
                    GameObject hudObj = Instantiate(hudPrefab);
                    hudObj.name = "GameHUD";
                    hudObj.transform.SetParent(transform, false);
                    gameHUD = hudObj.GetComponent<GameHUD>();
                    DontDestroyOnLoad(hudObj);
                    UnityEngine.Debug.Log("[GameManagers] Created GameHUD from prefab");
                }
                else
                {
                    UnityEngine.Debug.LogError("[GameManagers] Failed to load GameHUD prefab from Resources/Prefabs/UI/GameHUD");
                }
            }

            UnityEngine.Debug.Log("[GameManagers] State-dependent managers initialized");
            isInitialized = true;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
            
            // Save game state before quitting
            if (gameState != null)
            {
                gameState.ExportGameState();
            }
        }
    }
} 