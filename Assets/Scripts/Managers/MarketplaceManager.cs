using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace SMZero
{
    public class MarketplaceManager : MonoBehaviour
    {
        public static MarketplaceManager Instance { get; private set; }

        [Header("Character NFTs")]
        [SerializeField] private Sprite richardSprite;
        [SerializeField] private Sprite emilySprite;
        [SerializeField] private Sprite jakeSprite;

        [Header("Zone NFTs")]
        [SerializeField] private Sprite vaultAvenueSprite;

        private List<NFTDisplayData> listings = new List<NFTDisplayData>();
        private float playerBalance = 1000f; // Starting balance
        private PlayerInventory playerInventory = new PlayerInventory();

        public UnityEvent<float> OnBalanceChanged = new UnityEvent<float>();
        public UnityEvent<List<NFTDisplayData>> OnListingsUpdated = new UnityEvent<List<NFTDisplayData>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                ValidateSprites();
                InitializeMarketplace();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void ValidateSprites()
        {
            Debug.Log("Validating NFT sprites...");
            if (richardSprite == null)
            {
                Debug.LogError("Richard sprite is missing!");
                return;
            }
            if (emilySprite == null)
            {
                Debug.LogError("Emily sprite is missing!");
                return;
            }
            if (jakeSprite == null)
            {
                Debug.LogError("Jake sprite is missing!");
                return;
            }
            if (vaultAvenueSprite == null)
            {
                Debug.LogError("Vault Avenue sprite is missing!");
                return;
            }

            Debug.Log($"Sprite validation complete. Richard: {richardSprite.name}, Emily: {emilySprite.name}, Jake: {jakeSprite.name}, VaultAve: {vaultAvenueSprite.name}");
        }

        private void InitializeMarketplace()
        {
            Debug.Log("Initializing marketplace...");
            listings.Clear();

            // Add Character NFTs
            AddCharacterNFT("Richard", richardSprite, NFTRarity.Common, 400f, 1.0f, 1.0f);
            AddCharacterNFT("Emily", emilySprite, NFTRarity.Rare, 6000f, 1.5f, 1.2f);
            AddCharacterNFT("Jake", jakeSprite, NFTRarity.Epic, 12000f, 2.0f, 1.5f);

            // Add Zone NFT
            AddZoneNFT("Vault Avenue", vaultAvenueSprite, NFTRarity.Common, 500f);

            Debug.Log($"Marketplace initialized with {listings.Count} listings");
            foreach (var listing in listings)
            {
                if (listing.Type == NFTType.Character)
                {
                    var nft = (CharacterNFT)listing.NftData;
                    Debug.Log($"Character NFT: {nft.Name}, Price: {listing.Price}, Icon: {nft.Icon?.name ?? "null"}, Sprite valid: {nft.Icon != null}");
                }
                else
                {
                    var nft = (ZoneNFT)listing.NftData;
                    Debug.Log($"Zone NFT: {nft.Name}, Price: {listing.Price}, Icon: {nft.Icon?.name ?? "null"}, Sprite valid: {nft.Icon != null}");
                }
            }

            OnListingsUpdated.Invoke(GetAvailableListings());
            OnBalanceChanged.Invoke(playerBalance);
        }

        private void AddCharacterNFT(string name, Sprite icon, NFTRarity rarity, float price, float speedMod, float amountMod)
        {
            if (icon == null)
            {
                Debug.LogError($"Cannot add Character NFT {name}: icon is null!");
                return;
            }

            Debug.Log($"Creating Character NFT: {name} with sprite: {icon.name}");

            var nft = new CharacterNFT
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = name,
                Icon = icon,
                Rarity = rarity,
                SpeedModifier = speedMod,
                AmountModifier = amountMod,
                cost = price
            };

            var displayData = new NFTDisplayData
            {
                Id = nft.Id,
                Type = NFTType.Character,
                NftData = nft,
                Price = price,
                IsSold = false
            };

            listings.Add(displayData);

            // Verify the NFT data after creation
            var verifyNft = (CharacterNFT)displayData.NftData;
            Debug.Log($"Added Character NFT: {name}, Price: {price}, Icon: {verifyNft.Icon?.name ?? "null"}, Sprite valid: {verifyNft.Icon != null}");
        }

        private void AddZoneNFT(string name, Sprite icon, NFTRarity rarity, float price)
        {
            if (icon == null)
            {
                Debug.LogError($"Cannot add Zone NFT {name}: icon is null!");
                return;
            }

            Debug.Log($"Creating Zone NFT: {name} with sprite: {icon.name}");

            var nft = new ZoneNFT
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = name,
                Icon = icon,
                Rarity = rarity,
                cost = price
            };

            var displayData = new NFTDisplayData
            {
                Id = nft.Id,
                Type = NFTType.Zone,
                NftData = nft,
                Price = price,
                IsSold = false
            };

            listings.Add(displayData);

            // Verify the NFT data after creation
            var verifyNft = (ZoneNFT)displayData.NftData;
            Debug.Log($"Added Zone NFT: {name}, Price: {price}, Icon: {verifyNft.Icon?.name ?? "null"}, Sprite valid: {verifyNft.Icon != null}");
        }

        public List<NFTDisplayData> GetAvailableListings()
        {
            var available = listings.FindAll(l => !l.IsSold);
            Debug.Log($"Getting available listings: {available.Count} items");
            foreach (var listing in available)
            {
                if (listing.Type == NFTType.Character)
                {
                    var nft = (CharacterNFT)listing.NftData;
                    Debug.Log($"Available Character NFT: {nft.Name}, Icon: {nft.Icon?.name ?? "null"}, Sprite valid: {nft.Icon != null}");
                }
                else
                {
                    var nft = (ZoneNFT)listing.NftData;
                    Debug.Log($"Available Zone NFT: {nft.Name}, Icon: {nft.Icon?.name ?? "null"}, Sprite valid: {nft.Icon != null}");
                }
            }
            return available;
        }

        public Dictionary<string, NFTDisplayData> GetActiveListings()
        {
            var dict = new Dictionary<string, NFTDisplayData>();
            foreach (var listing in listings)
            {
                dict[listing.Id] = listing;
            }
            return dict;
        }

        public PlayerInventory GetPlayerInventory()
        {
            return playerInventory;
        }

        public void RestoreState(MarketplaceState state)
        {
            if (state == null)
            {
                Debug.LogWarning("Attempted to restore null marketplace state");
                return;
            }

            Debug.Log("Restoring marketplace state...");
            
            // Restore listings
            listings.Clear();
            foreach (var kvp in state.ActiveListings)
            {
                listings.Add(kvp.Value);
                if (kvp.Value.Type == NFTType.Character)
                {
                    var nft = (CharacterNFT)kvp.Value.NftData;
                    Debug.Log($"Restored Character NFT: {nft.Name}, Icon: {nft.Icon?.name ?? "null"}, Sprite valid: {nft.Icon != null}");
                }
                else
                {
                    var nft = (ZoneNFT)kvp.Value.NftData;
                    Debug.Log($"Restored Zone NFT: {nft.Name}, Icon: {nft.Icon?.name ?? "null"}, Sprite valid: {nft.Icon != null}");
                }
            }

            // Restore inventory
            playerInventory = state.PlayerInventory;

            // Notify UI
            OnListingsUpdated.Invoke(GetAvailableListings());
            OnBalanceChanged.Invoke(playerBalance);
            
            Debug.Log($"State restored with {listings.Count} listings");
        }

        public bool TryPurchaseNFT(NFTDisplayData listing)
        {
            if (listing.IsSold || playerBalance < listing.Price)
                return false;

            playerBalance -= listing.Price;
            listing.IsSold = true;

            // Update inventory
            if (listing.Type == NFTType.Character)
            {
                var nft = (CharacterNFT)listing.NftData;
                playerInventory.OwnedCharacterIds.Add(nft.Id);
                Debug.Log($"Purchased Character NFT: {nft.Name} for {listing.Price} FD");
            }
            else if (listing.Type == NFTType.Zone)
            {
                var nft = (ZoneNFT)listing.NftData;
                playerInventory.OwnedZoneIds.Add(nft.Id);
                Debug.Log($"Purchased Zone NFT: {nft.Name} for {listing.Price} FD");
            }

            OnBalanceChanged.Invoke(playerBalance);
            OnListingsUpdated.Invoke(GetAvailableListings());

            return true;
        }

        public float GetPlayerBalance()
        {
            return playerBalance;
        }
    }
} 