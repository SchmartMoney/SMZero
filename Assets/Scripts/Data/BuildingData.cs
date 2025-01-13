using UnityEngine;
using System.Collections.Generic;
using SMZero;

namespace SMZero
{
    [System.Serializable]
    public class BuildingData
    {
        [Header("State")]
        public bool isActive;
        
        [Header("Production")]
        public ProductionData productionSystem;
        
        [Header("Characters")]
        [SerializeField] private CharacterNFT[] characterSlots = new CharacterNFT[3];
        
        public bool CanAddCharacter => GetEmptySlotIndex() != -1;
        
        private int GetEmptySlotIndex()
        {
            for (int i = 0; i < characterSlots.Length; i++)
            {
                if (characterSlots[i] == null) return i;
            }
            return -1;
        }

        public bool AddCharacter(CharacterNFT character)
        {
            int emptySlot = GetEmptySlotIndex();
            if (emptySlot != -1)
            {
                characterSlots[emptySlot] = character;
                UpdateProductionValues();
                return true;
            }
            return false;
        }

        public bool RemoveCharacter(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < characterSlots.Length)
            {
                if (characterSlots[slotIndex] != null)
                {
                    characterSlots[slotIndex] = null;
                    UpdateProductionValues();
                    return true;
                }
            }
            return false;
        }

        private void UpdateProductionValues()
        {
            float timeMultiplier = 1f;
            float valueMultiplier = 1f;

            foreach (var character in characterSlots)
            {
                if (character != null)
                {
                    if (character.SpeedModifier > 0)
                        timeMultiplier *= (1f - character.SpeedModifier);
                    if (character.AmountModifier > 0)
                        valueMultiplier *= character.AmountModifier;
                }
            }

            if (productionSystem != null)
            {
                productionSystem.baseProductionTime *= timeMultiplier;
                productionSystem.baseAssetValue *= valueMultiplier;
            }
        }
    }
} 