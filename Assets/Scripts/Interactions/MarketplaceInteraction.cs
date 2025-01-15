using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace SMZero
{
    public class MarketplaceInteraction : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject marketplaceCanvas;
        [SerializeField] private Transform listingContainer;
        [SerializeField] private GameObject listingPrefab;
        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            Debug.Log("MarketplaceInteraction starting...");
            ValidateReferences();

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }

            if (MarketplaceManager.Instance != null)
            {
                MarketplaceManager.Instance.OnBalanceChanged.AddListener(UpdateBalanceText);
                MarketplaceManager.Instance.OnListingsUpdated.AddListener(OnListingsUpdated);
                UpdateUI();
            }
            else
            {
                Debug.LogError("MarketplaceManager not found!");
            }
        }

        private void ValidateReferences()
        {
            if (marketplaceCanvas == null) Debug.LogError("marketplaceCanvas is missing!");
            if (listingContainer == null) Debug.LogError("listingContainer is missing!");
            if (listingPrefab == null) Debug.LogError("listingPrefab is missing!");
            if (balanceText == null) Debug.LogError("balanceText is missing!");
            if (closeButton == null) Debug.LogError("closeButton is missing!");

            // Validate that the prefab has the ListingUI component
            var listingUI = listingPrefab?.GetComponent<ListingUI>();
            if (listingUI == null)
            {
                Debug.LogError("ListingUI component is missing from the prefab!");
            }
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void OnDisable()
        {
            if (MarketplaceManager.Instance != null)
            {
                MarketplaceManager.Instance.OnBalanceChanged.RemoveListener(UpdateBalanceText);
                MarketplaceManager.Instance.OnListingsUpdated.RemoveListener(OnListingsUpdated);
            }
        }

        private void UpdateUI()
        {
            UpdateBalanceDisplay();
            RefreshListings();
        }

        private void UpdateBalanceDisplay()
        {
            if (balanceText != null && MarketplaceManager.Instance != null)
            {
                UpdateBalanceText(MarketplaceManager.Instance.GetPlayerBalance());
            }
        }

        private void UpdateBalanceText(float balance)
        {
            if (balanceText != null)
            {
                balanceText.text = $"Balance: {balance:N0} FD";
            }
        }

        private void OnListingsUpdated(List<NFTDisplayData> listings)
        {
            RefreshListings();
        }

        private void RefreshListings()
        {
            if (listingContainer == null || MarketplaceManager.Instance == null) return;
            var listings = MarketplaceManager.Instance.GetAllListings();
            RefreshListings(listings);
        }

        private void RefreshListings(List<NFTDisplayData> listings)
        {
            if (listingContainer == null || MarketplaceManager.Instance == null) return;

            Debug.Log("Refreshing listings...");

            // Clear existing listings
            foreach (Transform child in listingContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new listing items
            foreach (var listing in listings)
            {
                try
                {
                    GameObject listingObj = Instantiate(listingPrefab, listingContainer);
                    ListingUI listingUI = listingObj.GetComponent<ListingUI>();
                    
                    if (listingUI != null)
                    {
                        Debug.Log($"Initializing UI for listing: {(listing.Type == NFTType.Character ? ((CharacterNFT)listing.NftData).Name : ((ZoneNFT)listing.NftData).Name)}");
                        listingUI.Initialize(listing, OnPurchaseClicked);
                    }
                    else
                    {
                        Debug.LogError($"ListingUI component not found on instantiated object!");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error creating listing UI: {e.Message}\n{e.StackTrace}");
                }
            }
        }

        private void OnPurchaseClicked(NFTDisplayData listing)
        {
            if (MarketplaceManager.Instance != null)
            {
                bool success = MarketplaceManager.Instance.TryPurchaseNFT(listing);
                if (success)
                {
                    UpdateUI();
                }
            }

            // Save state after purchase
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.ExportGameState();
            }
        }

        private void OnCloseClicked()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.ExportGameState();
            }

            if (marketplaceCanvas != null)
            {
                marketplaceCanvas.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }

            if (MarketplaceManager.Instance != null)
            {
                MarketplaceManager.Instance.OnBalanceChanged.RemoveListener(UpdateBalanceText);
                MarketplaceManager.Instance.OnListingsUpdated.RemoveListener(OnListingsUpdated);
            }
        }
    }
} 