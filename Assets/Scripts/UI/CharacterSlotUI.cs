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
        
        private void Awake()
        {
            if (stakeButton != null) stakeButton.onClick.AddListener(OnStakeClicked);
            if (unstakeButton != null) unstakeButton.onClick.AddListener(OnUnstakeClicked);
        }

        public void SetCharacter(string characterName, string rarity, bool isInteractable)
        {
            if (nameText != null) nameText.text = characterName;
            if (rarityText != null) rarityText.text = rarity;
            
            // Set alpha based on ownership
            if (canvasGroup != null)
            {
                canvasGroup.alpha = isInteractable ? 1f : 0.5f;
                canvasGroup.interactable = isInteractable;
            }
            
            // Update button interactability
            if (stakeButton != null) stakeButton.interactable = isInteractable;
            if (unstakeButton != null) unstakeButton.interactable = isInteractable;
            
            Debug.Log($"Character slot updated - Name: {characterName}, Rarity: {rarity}, Interactable: {isInteractable}");
        }
        
        private void OnStakeClicked()
        {
            Debug.Log("Character stake button clicked");
        }
        
        private void OnUnstakeClicked()
        {
            Debug.Log("Character unstake button clicked");
        }
        
        private void OnDestroy()
        {
            if (stakeButton != null) stakeButton.onClick.RemoveAllListeners();
            if (unstakeButton != null) unstakeButton.onClick.RemoveAllListeners();
        }
    }
} 