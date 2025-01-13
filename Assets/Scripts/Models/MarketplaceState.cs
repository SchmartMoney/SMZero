using System;
using System.Collections.Generic;

namespace SMZero
{
    [Serializable]
    public class MarketplaceState
    {
        public Dictionary<string, NFTDisplayData> ActiveListings;
        public PlayerInventory PlayerInventory;
    }

    [Serializable]
    public class PlayerInventory
    {
        public List<string> OwnedCharacterIds = new List<string>();
        public List<string> OwnedZoneIds = new List<string>();
        public List<string> ActiveListingIds = new List<string>();
    }
} 