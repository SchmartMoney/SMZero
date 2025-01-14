using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        public Button buyButton;

        private void Reset()
        {
            // This will run when the component is first added or reset in the inspector
            SetupComponents();
        }

        private void OnValidate()
        {
            // This will run in the editor whenever values change
            ValidateComponents();
            SetupImageComponent();
        }

        private void SetupComponents()
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
                buyButton = transform.Find("BuyButton")?.GetComponent<Button>();
            }

            // Only create a new image if one doesn't exist in the prefab
            if (itemImage == null)
            {
                Debug.LogWarning($"[{gameObject.name}] ItemImage not found in prefab, creating new one");
                GameObject imageObj = new GameObject("ItemImage", typeof(RectTransform));
                imageObj.transform.SetParent(transform, false);
                itemImage = imageObj.AddComponent<Image>();
            }

            SetupImageComponent();

            // Get the ListingUI component and set up its references
            var listingUI = GetComponent<ListingUI>();
            if (listingUI != null)
            {
                listingUI.SetupReferences(itemImage, nameText, rarityText, priceText, buyButton);
            }
        }

        private void SetupImageComponent()
        {
            if (itemImage != null)
            {
                // Configure the Image component
                itemImage.preserveAspect = true;
                itemImage.raycastTarget = false;
                
                // Get the RectTransform
                RectTransform rt = itemImage.GetComponent<RectTransform>();
                if (rt != null)
                {
                    // Set anchors to stretch
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(1, 1);
                    
                    // Reset position and size
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                    
                    // Set as first sibling so it's behind other elements
                    rt.SetAsFirstSibling();
                }
            }
        }

        private void ValidateComponents()
        {
            if (itemImage == null) Debug.LogError($"[{gameObject.name}] Item Image is missing!");
            if (nameText == null) Debug.LogError($"[{gameObject.name}] Name Text is missing!");
            if (rarityText == null) Debug.LogError($"[{gameObject.name}] Rarity Text is missing!");
            if (priceText == null) Debug.LogError($"[{gameObject.name}] Price Text is missing!");
            if (buyButton == null) Debug.LogError($"[{gameObject.name}] Buy Button is missing!");
        }

        private void Start()
        {
            // Ensure proper setup at runtime
            SetupImageComponent();
        }
    }
} 