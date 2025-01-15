using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SMZero
{
    public class ZoneUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI zoneNameText;
        [SerializeField] private Button returnToMapButton;
        [SerializeField] private BuildingPopup buildingPopup;
        
        [Header("Static Slots")]
        [SerializeField] private BuildingSlotUI[] buildingSlots;
        
        private ZoneManager zoneManager;
        
        private void Awake()
        {
            ValidateReferences();
            SetupUI();
        }
        
        private void ValidateReferences()
        {
            if (zoneNameText == null) Debug.LogError("Zone name text is missing!");
            if (returnToMapButton == null) Debug.LogError("Return to map button is missing!");
            if (buildingPopup == null) Debug.LogError("Building popup is missing!");
            if (buildingSlots == null || buildingSlots.Length == 0) Debug.LogError("Building slots array is empty!");
        }
        
        private void SetupUI()
        {
            // Get zone manager reference
            zoneManager = FindObjectOfType<ZoneManager>();
            if (zoneManager == null)
            {
                Debug.LogError("ZoneManager not found in scene!");
                return;
            }
            
            // Setup zone name
            zoneNameText.text = zoneManager.ZoneName;
            
            // Setup return button
            returnToMapButton.onClick.AddListener(OnReturnToMapClicked);
            
            // Initialize building slots
            InitializeSlots();

            // Ensure popup is hidden at start
            if (buildingPopup != null)
            {
                buildingPopup.Hide();
            }
        }
        
        public void InitializeSlots()
        {
            var buildings = zoneManager.GetBuildings();
            var marketplaceManager = MarketplaceManager.Instance;
            bool ownsVaultAvenue = false;

            // Check if player owns Vault Avenue
            if (marketplaceManager != null)
            {
                var ownedZones = marketplaceManager.GetOwnedZoneNFTs();
                ownsVaultAvenue = ownedZones?.Find(z => z.Id == "vault-avenue-001") != null;
            }
            
            // Initialize each slot with its corresponding building
            for (int i = 0; i < buildingSlots.Length && i < buildings.Length; i++)
            {
                var slot = buildingSlots[i];
                var building = buildings[i];
                
                if (slot != null && building != null)
                {
                    // First slot (Vault Avenue) is special
                    bool isFirstSlot = i == 0;
                    bool isEnabled = isFirstSlot && ownsVaultAvenue;
                    
                    // Initialize the slot
                    slot.Initialize(isEnabled);
                    slot.SetNFTText(isFirstSlot ? "Vault Avenue" : $"Building {i + 1}");
                }
            }
        }
        
        private void OnReturnToMapClicked()
        {
            SceneManager.LoadScene("MainScene");
        }
        
        private void OnDestroy()
        {
            if (returnToMapButton != null)
            {
                returnToMapButton.onClick.RemoveAllListeners();
            }
        }

        public BuildingPopup GetBuildingPopup()
        {
            if (buildingPopup == null)
            {
                Debug.LogError("BuildingPopup reference is null in ZoneUI!");
            }
            return buildingPopup;
        }
    }
} 