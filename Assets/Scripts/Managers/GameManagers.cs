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

        public DebugOverlay Debug => debugOverlay;
        public GameHUD HUD => gameHUD;
        public GameStateManager GameState => gameState;
        public PlayerProgress Progress => playerProgress;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Create managers if not set
            if (debugOverlay == null)
            {
                GameObject debugObj = new GameObject("DebugOverlay");
                debugObj.transform.SetParent(transform);
                debugOverlay = debugObj.AddComponent<DebugOverlay>();
            }

            if (gameHUD == null)
            {
                GameObject hudObj = new GameObject("GameHUD");
                hudObj.transform.SetParent(transform);
                gameHUD = hudObj.AddComponent<GameHUD>();
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
        }
    }
} 