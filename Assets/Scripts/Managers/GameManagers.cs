using UnityEngine;

namespace SMZero
{
    public class GameManagers : MonoBehaviour
    {
        private static GameManagers instance;
        public static GameManagers Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManagers");
                    instance = go.AddComponent<GameManagers>();
                    DontDestroyOnLoad(go);
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
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeManagers();
        }

        private void InitializeManagers()
        {
            // Create managers if not set
            if (debugOverlay == null)
            {
                GameObject debugObj = new GameObject("DebugOverlay");
                debugObj.transform.SetParent(transform);
                debugOverlay = debugObj.AddComponent<DebugOverlay>();
            }

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
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to load GameHUD prefab from Resources/Prefabs/UI/GameHUD");
                }
            }

            if (gameState == null)
            {
                GameObject stateObj = new GameObject("GameState");
                stateObj.transform.SetParent(transform);
                gameState = stateObj.AddComponent<GameStateManager>();
            }

            if (playerProgress == null)
            {
                GameObject progressObj = new GameObject("PlayerProgress");
                progressObj.transform.SetParent(transform);
                playerProgress = progressObj.AddComponent<PlayerProgress>();
            }

            if (marketplace == null)
            {
                GameObject marketplaceObj = new GameObject("Marketplace");
                marketplaceObj.transform.SetParent(transform);
                marketplace = marketplaceObj.AddComponent<MarketplaceManager>();
            }
        }
    }
} 