using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace SMZero
{
    public class ListingUI : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI rarityText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button buyButton;

        private NFTDisplayData listing;
        private Action<NFTDisplayData> onPurchaseClicked;

        public void SetupReferences(Image image, TextMeshProUGUI name, TextMeshProUGUI rarity, TextMeshProUGUI price, Button buy)
        {
            itemImage = image;
            nameText = name;
            rarityText = rarity;
            priceText = price;
            buyButton = buy;

            if (itemImage != null)
            {
                ConfigureImageComponent();
            }
        }

        private void ConfigureImageComponent()
        {
            itemImage.preserveAspect = true;
            itemImage.raycastTarget = false;
            
            var rt = itemImage.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                rt.SetAsFirstSibling();
            }
        }

        private void OnValidate()
        {
            ValidateReferences();
            if (itemImage != null)
            {
                ConfigureImageComponent();
            }
        }

        private void ValidateReferences()
        {
            if (itemImage == null) Debug.LogError($"[{gameObject.name}] itemImage is missing!");
            if (nameText == null) Debug.LogError($"[{gameObject.name}] nameText is missing!");
            if (rarityText == null) Debug.LogError($"[{gameObject.name}] rarityText is missing!");
            if (priceText == null) Debug.LogError($"[{gameObject.name}] priceText is missing!");
            if (buyButton == null) Debug.LogError($"[{gameObject.name}] buyButton is missing!");
        }

        public void Initialize(NFTDisplayData listingData, Action<NFTDisplayData> purchaseCallback)
        {
            if (listingData == null)
            {
                Debug.LogError($"[{gameObject.name}] Received null listing data!");
                return;
            }

            listing = listingData;
            onPurchaseClicked = purchaseCallback;

            ValidateReferences();

            // Set up UI elements based on NFT type
            string name = "";
            string rarity = "";

            try
            {
                if (listing.Type == NFTType.Character)
                {
                    var characterData = (CharacterNFT)listing.NftData;
                    name = characterData.Name;
                    rarity = characterData.Rarity.ToString();
                    if (itemImage != null)
                    {
                        itemImage.sprite = characterData.Icon;
                        itemImage.enabled = true;
                        Debug.Log($"[{gameObject.name}] Set character icon: {characterData.Name}, Icon null? {characterData.Icon == null}, Image enabled? {itemImage.enabled}");
                    }
                }
                else if (listing.Type == NFTType.Zone)
                {
                    var zoneData = (ZoneNFT)listing.NftData;
                    name = zoneData.Name;
                    rarity = zoneData.Rarity.ToString();
                    if (itemImage != null)
                    {
                        itemImage.sprite = zoneData.Icon;
                        itemImage.enabled = true;
                        Debug.Log($"[{gameObject.name}] Set zone icon: {zoneData.Name}, Icon null? {zoneData.Icon == null}, Image enabled? {itemImage.enabled}");
                    }
                }

                if (nameText != null) nameText.text = name;
                if (rarityText != null) rarityText.text = $"({rarity})";
                if (priceText != null) priceText.text = $"{listing.Price} FD";

                Debug.Log($"[{gameObject.name}] Initialized listing: {name}, {rarity}, {listing.Price} FD");
            }
            catch (Exception e)
            {
                Debug.LogError($"[{gameObject.name}] Error initializing listing: {e.Message}\n{e.StackTrace}");
            }
            
            // Set up button listener
            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnBuyClicked);
            }
        }

        private void OnBuyClicked()
        {
            onPurchaseClicked?.Invoke(listing);
        }
    }
} 