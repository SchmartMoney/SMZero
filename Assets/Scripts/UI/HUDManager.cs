using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SMZero
{
    public class HUDManager : MonoBehaviour
    {
        [Header("Player Info")]
        [SerializeField] private Image playerPortrait;
        [SerializeField] private TextMeshProUGUI playerIdText;
        [SerializeField] private Slider progressBar;

        [Header("Location")]
        [SerializeField] private TextMeshProUGUI locationText;

        [Header("Balance")]
        [SerializeField] private Image fdIcon;
        [SerializeField] private TextMeshProUGUI balanceText;

        [Header("Buttons")]
        [SerializeField] private Button speedUpButton;
        [SerializeField] private Button resetStateButton;

        private void Awake()
        {
            // Subscribe to balance updates as early as possible
            if (PlayerProgress.Instance != null)
            {
                PlayerProgress.Instance.OnBalanceChanged += UpdateBalance;
            }
        }

        private void Start()
        {
            InitializeUI();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Re-subscribe to balance updates when enabled
            if (PlayerProgress.Instance != null)
            {
                PlayerProgress.Instance.OnBalanceChanged += UpdateBalance;
                // Force an immediate balance update
                UpdateBalance(PlayerProgress.Instance.GetFortuneDollars());
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (PlayerProgress.Instance != null)
            {
                PlayerProgress.Instance.OnBalanceChanged -= UpdateBalance;
            }
        }

        private void Update()
        {
            // Continuously check and update balance
            if (PlayerProgress.Instance != null)
            {
                UpdateBalance(PlayerProgress.Instance.GetFortuneDollars());
            }
        }

        private void InitializeUI()
        {
            // Set initial player info
            if (playerIdText != null)
            {
                playerIdText.text = "Player #1234";
            }

            // Setup button listeners
            if (speedUpButton != null)
            {
                speedUpButton.onClick.RemoveAllListeners();
                speedUpButton.onClick.AddListener(OnSpeedUpClicked);
            }
            else
            {
                Debug.LogError("Speed Up button reference missing in HUDManager!");
            }

            if (resetStateButton != null)
            {
                resetStateButton.onClick.RemoveAllListeners();
                resetStateButton.onClick.AddListener(OnResetClicked);
            }
            else
            {
                Debug.LogError("Reset State button reference missing in HUDManager!");
            }

            // Set initial location
            UpdateLocationForCurrentScene();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateLocationForCurrentScene();
        }

        private void UpdateLocationForCurrentScene()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case "MainScene":
                    SetLocation("Willow's Creek");
                    break;
                case "VaultAvenueDetailedScene":
                    SetLocation("Vault Avenue");
                    break;
                default:
                    SetLocation(string.Empty);
                    break;
            }
        }

        private void OnSpeedUpClicked()
        {
            Debug.Log("Speed Up button clicked");
            var zoneManager = ZoneManager.Instance;
            if (zoneManager == null)
            {
                Debug.LogWarning("ZoneManager not found!");
                return;
            }

            var activeBuilding = zoneManager.GetActiveBuilding();
            if (activeBuilding == null)
            {
                Debug.LogWarning("No active building selected!");
                return;
            }

            var buildingPopup = FindObjectOfType<BuildingPopup>();
            if (buildingPopup != null)
            {
                buildingPopup.SpeedUp();
                Debug.Log("Production time reduced to 5 seconds remaining");
            }
            else
            {
                Debug.LogWarning("BuildingPopup not found!");
            }
        }

        private void OnResetClicked()
        {
            Debug.Log("Reset State button clicked");
            if (GameStateManager.Instance != null)
            {
                // Delete local storage
                PlayerPrefs.DeleteKey(GameStateManager.LOCAL_SAVE_KEY);
                PlayerPrefs.Save();
                
                // Reset player progress
                if (PlayerProgress.Instance != null)
                {
                    PlayerProgress.Instance.SetFortuneDollars(1000f);
                }
                
                // Create new game state
                GameStateManager.Instance.ResetState();
                
                Debug.Log("State has been reset to defaults");
                Debug.Log("Please restart the game to apply changes");
            }
            else
            {
                Debug.LogError("GameStateManager not found!");
            }
        }

        private void UpdateBalance(float newBalance)
        {
            if (balanceText != null)
            {
                balanceText.text = $"{newBalance:N2} FD";
            }
            else
            {
                Debug.LogError("Balance text reference missing in HUDManager!");
            }
        }

        private void SetLocation(string locationName)
        {
            if (locationText != null)
            {
                locationText.text = locationName;
            }
            else
            {
                Debug.LogError("Location text reference missing in HUDManager!");
            }
        }

        private void OnDestroy()
        {
            if (speedUpButton != null)
            {
                speedUpButton.onClick.RemoveListener(OnSpeedUpClicked);
            }

            if (resetStateButton != null)
            {
                resetStateButton.onClick.RemoveListener(OnResetClicked);
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (PlayerProgress.Instance != null)
            {
                PlayerProgress.Instance.OnBalanceChanged -= UpdateBalance;
            }
        }
    }
} 