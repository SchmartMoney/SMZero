using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SMZero
{
    public class BuildingPopup : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI buildingNameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private Button closeButton;

        private Building currentBuilding;

        private void Awake()
        {
            ValidateComponents();
            closeButton.onClick.AddListener(Hide);
        }

        private void ValidateComponents()
        {
            if (buildingNameText == null)
                Debug.LogError($"BuildingPopup {gameObject.name} is missing buildingNameText reference!");
            if (descriptionText == null)
                Debug.LogError($"BuildingPopup {gameObject.name} is missing descriptionText reference!");
            if (popupPanel == null)
                Debug.LogError($"BuildingPopup {gameObject.name} is missing popupPanel reference!");
            if (closeButton == null)
                Debug.LogError($"BuildingPopup {gameObject.name} is missing closeButton reference!");
        }

        public void Show(Building building)
        {
            if (building == null)
            {
                Debug.LogError("Attempting to show popup for null building!");
                return;
            }

            currentBuilding = building;
            UpdateUI();
            popupPanel.SetActive(true);
        }

        public void Hide()
        {
            currentBuilding = null;
            popupPanel.SetActive(false);
        }

        private void UpdateUI()
        {
            if (currentBuilding == null) return;

            buildingNameText.text = currentBuilding.name;
            
            string status = currentBuilding.IsActive ? "Active" : "Inactive";
            string production = currentBuilding.IsProducing ? "Producing" : "Not Producing";
            string characters = $"Characters: {currentBuilding.StakedCharacters.Count}/3";
            
            descriptionText.text = $"Status: {status}\n{production}\n{characters}";
        }
    }
} 