using UnityEngine;
using System;

namespace SMZero
{
    public enum NFTRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    [System.Serializable]
    public class ZoneNFT
    {
        public string Id;
        public string Name;
        public string MarketplaceName;
        public Sprite Icon;
        public NFTRarity Rarity;

        [Header("Economy")]
        public float cost = 500f;

        [Header("State")]
        public bool isActive;
        
        [NonSerialized] public string associatedBuildingId;
    }

    [System.Serializable]
    public class CharacterNFT
    {
        public string Id;
        public string Name;
        public Sprite Icon;
        public NFTRarity Rarity;
        public float SpeedModifier;
        public float AmountModifier;

        [Header("Economy")]
        public float cost;
        public float baseIncomePerDay;

        [Header("Progress")]
        public float progressRate;

        [NonSerialized] public float accumulatedIncome;
        [NonSerialized] public DateTime lastCollectionTime;

        private void OnValidate()
        {
            progressRate = baseIncomePerDay / (24f * 60f * 60f);
        }

        public void InitializeRuntime()
        {
            accumulatedIncome = 0f;
            lastCollectionTime = DateTime.Now;
        }
    }

    [System.Serializable]
    public class ProductionData
    {
        public float baseProductionTime = 1f;
        public float baseAssetValue = 50f;

        [NonSerialized] public float currentProgress;
        [NonSerialized] public float targetProgress;
        [NonSerialized] public bool isProducing;
        [NonSerialized] public DateTime lastProductionTime;
    }
} 