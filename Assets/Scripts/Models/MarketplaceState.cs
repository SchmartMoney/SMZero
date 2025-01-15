using System;
using System.Collections.Generic;

namespace SMZero
{
    [Serializable]
    public class MarketplaceState
    {
        public Dictionary<string, NFTDisplayData> ActiveListings;
        public PlayerInventory PlayerInventory { get; set; }
        public float PlayerBalance { get; set; }
    }

    [Serializable]
    public class PlayerInventory
    {
        public List<string> OwnedCharacterIds { get; set; } = new List<string>();
        public List<string> OwnedZoneIds { get; set; } = new List<string>();
        public List<string> ActiveListingIds = new List<string>();
    }
} 