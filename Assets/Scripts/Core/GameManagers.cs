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
                    var go = new GameObject("GameManagers");
                    instance = go.AddComponent<GameManagers>();
                }
                return instance;
            }
        }

        public GameStateManager GameState { get; private set; }
        public DebugOverlay Debug { get; private set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Create child objects for managers
            var gameStateObj = new GameObject("GameState");
            gameStateObj.transform.SetParent(transform);
            GameState = gameStateObj.AddComponent<GameStateManager>();

            var debugObj = new GameObject("Debug");
            debugObj.transform.SetParent(transform);
            Debug = debugObj.AddComponent<DebugOverlay>();
        }
    }
} 