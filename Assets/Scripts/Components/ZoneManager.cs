using UnityEngine;
using UnityEngine.SceneManagement;

namespace SMZero
{
    public class ZoneManager : MonoBehaviour
    {
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
            DontDestroyOnLoad(gameObject);

            zoneUI = FindObjectOfType<ZoneUI>();
            buildingPopup = FindObjectOfType<BuildingPopup>();
            gameStateManager = GameStateManager.Instance;
        }

        private void Start()
        {
            if (gameStateManager != null)
            {
                var gameState = gameStateManager.GetCurrentState();
                if (gameState != null)
                {
                    Debug.Log("=== Game State Loaded ===");
                    Debug.Log($"Player Balance: {gameState.PlayerBalance}");
                    if (gameState.PlayerInventory != null)
                    {
                        Debug.Log($"Player Inventory - Character IDs: {gameState.PlayerInventory?.OwnedCharacterIds?.Count ?? 0}");
                        Debug.Log($"Player Inventory - Zone IDs: {gameState.PlayerInventory?.OwnedZoneIds?.Count ?? 0}");
                    }
                    Debug.Log("=====================");
                }
                else
                {
                    Debug.LogWarning("No game state found!");
                }
            }
            else
            {
                Debug.LogError("GameStateManager not found!");
            }
        }

        public void StakeZoneNFT(ZoneNFT zoneNFT)
        {
            if (activeBuilding == null)
            {
                Debug.LogError("No active building to stake Zone NFT to!");
                return;
            }

            activeZoneNFT = zoneNFT;
            activeBuilding.SetActive(true);
            
            if (zoneUI != null)
            {
                zoneUI.InitializeSlots();
            }
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
        }

        public void SetActiveBuilding(Building building)
        {
            activeBuilding = building;
            if (buildingPopup != null)
            {
                buildingPopup.Show(building);
            }
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