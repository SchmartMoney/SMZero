using UnityEngine;
using System.Collections.Generic;

namespace SMZero
{
    public class Building : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float baseProductionTime = 10f; // 10 seconds for testing
        [SerializeField] private int baseFortuneAmount = 100;

        [Header("Colors")]
        [SerializeField] private Color activeColor = Color.yellow;
        [SerializeField] private Color inactiveColor = Color.gray;
        
        private bool isActive;
        private List<CharacterNFT> stakedCharacters = new List<CharacterNFT>();
        private float currentProductionTime;
        private bool isProducing;
        private bool canCollect;
        private LocationHighlight locationHighlight;

        // Public properties for UI
        public bool IsActive => isActive;
        public bool IsProducing => isProducing;
        public bool CanCollect => canCollect;
        public float CurrentProductionTime => currentProductionTime;
        public IReadOnlyList<CharacterNFT> StakedCharacters => stakedCharacters.AsReadOnly();
        public int BaseFortuneAmount => baseFortuneAmount;

        private void Awake()
        {
            locationHighlight = GetComponent<LocationHighlight>();
            if (locationHighlight == null)
            {
                Debug.LogError($"Building {gameObject.name} is missing LocationHighlight component!");
            }
        }

        public void SetActive(bool active)
        {
            isActive = active;
            if (locationHighlight != null)
            {
                locationHighlight.SetHighlightColor(active ? activeColor : inactiveColor);
                locationHighlight.SetInteractable(active);
            }
        }

        public bool CanAddCharacter()
        {
            return isActive && stakedCharacters.Count < 3;
        }

        public void StakeCharacter(CharacterNFT character)
        {
            if (!isActive)
            {
                Debug.Log("Cannot stake character - building not active (needs Zone NFT)");
                return;
            }

            if (stakedCharacters.Count >= 3)
            {
                Debug.Log("Cannot stake character - maximum number of characters reached");
                return;
            }
            
            stakedCharacters.Add(character);
            UpdateProductionModifiers();
            
            // Start production when first character is staked
            if (!isProducing && stakedCharacters.Count == 1)
            {
                StartProduction();
            }
        }

        public void UnstakeCharacter(CharacterNFT character)
        {
            if (stakedCharacters.Remove(character))
            {
                Debug.Log($"Character unstaked from building {gameObject.name}");
                UpdateProductionModifiers();

                // Stop production if no characters left
                if (stakedCharacters.Count == 0)
                {
                    StopProduction();
                }
            }
        }

        private void StartProduction()
        {
            if (!isActive || stakedCharacters.Count == 0)
            {
                return;
            }

            isProducing = true;
            currentProductionTime = 0f;
            canCollect = false;
        }

        private void StopProduction()
        {
            isProducing = false;
            currentProductionTime = 0f;
            canCollect = false;
        }

        private void UpdateProductionModifiers()
        {
            // Calculate production modifiers based on staked characters
            float speedModifier = 1f;
            float amountModifier = 1f;

            foreach (var character in stakedCharacters)
            {
                speedModifier += character.SpeedModifier;
                amountModifier += character.AmountModifier;
            }

            // Apply modifiers to production values
            currentProductionTime = baseProductionTime / speedModifier;
            baseFortuneAmount = Mathf.RoundToInt(baseFortuneAmount * amountModifier);
        }
    }
} 