using UnityEngine;

namespace SMZero
{
    public enum NFTType
    {
        Character,
        Zone
    }

    [System.Serializable]
    public class NFTDisplayData
    {
        public string Id;
        public NFTType Type;
        public object NftData;
        public float Price;
        public bool IsSold;
    }
} 