using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SMZero
{
    public class ZoneUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI zoneNameText;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform slotsContainer;

        private void Awake()
        {
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (zoneNameText == null)
                Debug.LogError($"ZoneUI {gameObject.name} is missing zoneNameText reference!");
            if (slotPrefab == null)
                Debug.LogError($"ZoneUI {gameObject.name} is missing slotPrefab reference!");
            if (slotsContainer == null)
                Debug.LogError($"ZoneUI {gameObject.name} is missing slotsContainer reference!");
        }

        public void InitializeSlots()
        {
            // Clear existing slots
            foreach (Transform child in slotsContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new slot
            var slot = Instantiate(slotPrefab, slotsContainer);
            var zoneNFT = ZoneManager.Instance.GetActiveZoneNFT();
            if (zoneNFT != null)
            {
                UpdateZoneSlot(zoneNFT);
            }
        }

        public void UpdateZoneSlot(ZoneNFT nft)
        {
            if (nft == null)
            {
                zoneNameText.text = "Empty Slot";
                return;
            }

            zoneNameText.text = nft.Name;
        }
    }
} 