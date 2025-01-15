using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMZero
{
    [Serializable]
    public class MarketplaceState
    {
        [JsonProperty("l")]
        public Dictionary<string, NFTDisplayData> ActiveListings;
        
        [JsonProperty("i")]
        public PlayerInventory PlayerInventory { get; set; }
        
        [JsonProperty("b")]
        public float PlayerBalance { get; set; }
    }

    [Serializable]
    public class PlayerInventory
    {
        [JsonProperty("c")]
        public List<string> OwnedCharacterIds { get; set; } = new List<string>();
        
        [JsonProperty("z")]
        public List<string> OwnedZoneIds { get; set; } = new List<string>();
        
        [JsonProperty("a")]
        public List<string> ActiveListingIds = new List<string>();
    }
} 