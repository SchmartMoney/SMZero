using UnityEngine;
using Newtonsoft.Json;

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
        [JsonProperty("i")]
        public string Id;
        
        [JsonProperty("t")]
        public NFTType Type;
        
        [JsonProperty("d")]
        public object NftData;
        
        [JsonProperty("p")]
        public float Price;
        
        [JsonProperty("s")]
        public bool IsSold;
    }
} 