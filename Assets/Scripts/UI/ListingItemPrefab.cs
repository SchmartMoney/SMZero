using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UltimateClean;

namespace SMZero
{
    /// <summary>
    /// Helper component to ensure proper prefab setup
    /// </summary>
    [RequireComponent(typeof(ListingUI))]
    public class ListingItemPrefab : MonoBehaviour
    {
        [Header("Required Components")]
        public Image itemImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI priceText;
        public CleanButton buyButton;

        private void Reset()
        {
            // This will run when the component is first added or reset in the inspector
            ValidateComponents();
        }

        private void OnValidate()
        {
            // This will run in the editor whenever values change
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            // Try to find existing components by name first
            if (itemImage == null)
            {
                itemImage = transform.Find("ItemImage")?.GetComponent<Image>();
            }
            if (nameText == null)
            {
                nameText = transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            }
            if (rarityText == null)
            {
                rarityText = transform.Find("RarityText")?.GetComponent<TextMeshProUGUI>();
            }
            if (priceText == null)
            {
                priceText = transform.Find("PriceText")?.GetComponent<TextMeshProUGUI>();
            }
            if (buyButton == null)
            {
                buyButton = transform.Find("BuyButton")?.GetComponent<CleanButton>();
            }

            // Get the ListingUI component and set up its references
            var listingUI = GetComponent<ListingUI>();
            if (listingUI != null)
            {
                listingUI.SetupReferences(itemImage, nameText, rarityText, priceText, buyButton);
            }
        }
    }
} 