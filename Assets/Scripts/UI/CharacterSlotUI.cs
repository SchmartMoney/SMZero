using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SMZero
{
    public class CharacterSlotUI : MonoBehaviour
    {
        public Image characterIcon;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI rarityText;
        public Button stakeButton;
        public Button unstakeButton;
        public CanvasGroup canvasGroup;

        private string characterId;
        private Building currentBuilding;
        
        private void Awake()
        {
            if (stakeButton != null) stakeButton.onClick.AddListener(OnStakeClicked);
            if (unstakeButton != null) unstakeButton.onClick.AddListener(OnUnstakeClicked);
        }

        public void SetCharacter(string characterName, string rarity, bool isInteractable)
        {
            if (nameText != null) nameText.text = characterName;
            if (rarityText != null) rarityText.text = rarity;
            
            // Set character ID based on name
            switch (characterName)
            {
                case "Richard":
                    characterId = "richard-nft-001";
                    break;
                case "Emily":
                    characterId = "emily-nft-001";
                    break;
                case "Jake":
                    characterId = "jake-nft-001";
                    break;
            }
            
            // Set alpha based on ownership
            if (canvasGroup != null)
            {
                canvasGroup.alpha = isInteractable ? 1f : 0.5f;
                canvasGroup.interactable = isInteractable;
            }
            
            // Update button interactability
            if (stakeButton != null) stakeButton.interactable = isInteractable;
            if (unstakeButton != null) unstakeButton.interactable = isInteractable;
            
            // Get current building reference
            currentBuilding = ZoneManager.Instance?.GetActiveBuilding();
            
            // Update button visibility based on staking status
            UpdateButtonVisibility();
            
            Debug.Log($"Character slot updated - Name: {characterName}, ID: {characterId}, Rarity: {rarity}, Interactable: {isInteractable}");
        }
        
        private void UpdateButtonVisibility()
        {
            if (currentBuilding == null || string.IsNullOrEmpty(characterId)) return;

            bool isStaked = currentBuilding.GetStakedCharacterIds().Contains(characterId);
            
            if (stakeButton != null)
            {
                stakeButton.gameObject.SetActive(!isStaked);
            }
            
            if (unstakeButton != null)
            {
                unstakeButton.gameObject.SetActive(isStaked);
            }
        }
        
        private void OnStakeClicked()
        {
            Debug.Log($"Character stake button clicked for {characterId}");
            
            if (currentBuilding == null || string.IsNullOrEmpty(characterId))
            {
                Debug.LogError("Cannot stake character: building or character ID is null");
                return;
            }

            var marketplaceManager = MarketplaceManager.Instance;
            if (marketplaceManager != null)
            {
                var character = marketplaceManager.GetCharacterNFTById(characterId);
                if (character != null)
                {
                    currentBuilding.StakeCharacter(character);
                    UpdateButtonVisibility();
                }
                else
                {
                    Debug.LogError($"Character NFT not found for ID: {characterId}");
                }
            }
        }
        
        private void OnUnstakeClicked()
        {
            Debug.Log($"Character unstake button clicked for {characterId}");
            
            if (currentBuilding == null || string.IsNullOrEmpty(characterId))
            {
                Debug.LogError("Cannot unstake character: building or character ID is null");
                return;
            }

            var marketplaceManager = MarketplaceManager.Instance;
            if (marketplaceManager != null)
            {
                var character = marketplaceManager.GetCharacterNFTById(characterId);
                if (character != null)
                {
                    currentBuilding.UnstakeCharacter(character);
                    UpdateButtonVisibility();
                }
                else
                {
                    Debug.LogError($"Character NFT not found for ID: {characterId}");
                }
            }
        }
        
        private void OnDestroy()
        {
            if (stakeButton != null) stakeButton.onClick.RemoveAllListeners();
            if (unstakeButton != null) unstakeButton.onClick.RemoveAllListeners();
        }
    }
} 