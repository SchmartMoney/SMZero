using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Linq;

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
            OnBalanceChanged.Invoke(PlayerProgress.Instance.GetFortuneDollars());
        }

        private void AddCharacterNFT(string name, Sprite icon, NFTRarity rarity, float price, float speedMod, float amountMod)
        {
            if (icon == null)
            {
                Debug.LogError($"Cannot add Character NFT {name}: icon is null!");
                return;
            }

            Debug.Log($"Creating Character NFT: {name} with sprite: {icon.name}");

            // Use consistent IDs based on name
            string id;
            switch (name)
            {
                case "Richard":
                    id = "richard-nft-001";
                    break;
                case "Emily":
                    id = "emily-nft-001";
                    break;
                case "Jake":
                    id = "jake-nft-001";
                    break;
                default:
                    id = System.Guid.NewGuid().ToString();
                    break;
            }

            var nft = new CharacterNFT
            {
                Id = id,
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

            // Check if this NFT was already purchased
            if (playerInventory.OwnedCharacterIds.Contains(nft.Id))
            {
                displayData.IsSold = true;
            }

            listings.Add(displayData);

            // Verify the NFT data after creation
            var verifyNft = (CharacterNFT)displayData.NftData;
            Debug.Log($"Added Character NFT: {name}, ID: {id}, Price: {price}, Icon: {verifyNft.Icon?.name ?? "null"}, Sprite valid: {verifyNft.Icon != null}, IsSold: {displayData.IsSold}");
        }

        private void AddZoneNFT(string name, Sprite icon, NFTRarity rarity, float price)
        {
            if (icon == null)
            {
                Debug.LogError($"Cannot add Zone NFT {name}: icon is null!");
                return;
            }

            Debug.Log($"Creating Zone NFT: {name} with sprite: {icon.name}");

            // Use consistent IDs based on name
            string id;
            switch (name)
            {
                case "Vault Avenue":
                    id = "vault-avenue-001";
                    break;
                default:
                    id = System.Guid.NewGuid().ToString();
                    break;
            }

            var nft = new ZoneNFT
            {
                Id = id,
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

            // Check if this NFT was already purchased
            if (playerInventory.OwnedZoneIds.Contains(nft.Id))
            {
                displayData.IsSold = true;
            }

            listings.Add(displayData);

            // Verify the NFT data after creation
            var verifyNft = (ZoneNFT)displayData.NftData;
            Debug.Log($"Added Zone NFT: {name}, ID: {id}, Price: {price}, Icon: {verifyNft.Icon?.name ?? "null"}, Sprite valid: {verifyNft.Icon != null}, IsSold: {displayData.IsSold}");
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
                Debug.LogWarning("Attempted to restore null marketplace state, initializing new state");
                playerInventory = new PlayerInventory();
                return;
            }

            Debug.Log("Restoring marketplace state...");
            
            // Restore inventory first
            playerInventory = state.PlayerInventory ?? new PlayerInventory();

            // Update sold status of listings based on inventory
            foreach (var listing in listings)
            {
                if (listing?.NftData == null) continue;

                if (listing.Type == NFTType.Character)
                {
                    var nft = (CharacterNFT)listing.NftData;
                    listing.IsSold = playerInventory.OwnedCharacterIds?.Contains(nft.Id) ?? false;
                }
                else if (listing.Type == NFTType.Zone)
                {
                    var nft = (ZoneNFT)listing.NftData;
                    listing.IsSold = playerInventory.OwnedZoneIds?.Contains(nft.Id) ?? false;
                }
            }

            // Notify UI
            OnListingsUpdated?.Invoke(GetAvailableListings());
            if (PlayerProgress.Instance != null)
            {
                OnBalanceChanged?.Invoke(PlayerProgress.Instance.GetFortuneDollars());
            }
            
            Debug.Log($"State restored with {listings.Count} listings");
        }

        public bool TryPurchaseNFT(NFTDisplayData listing)
        {
            if (listing == null || listing.NftData == null || PlayerProgress.Instance == null)
            {
                Debug.LogError("Cannot purchase NFT: null reference detected");
                return false;
            }

            if (listing.IsSold || !PlayerProgress.Instance.SpendFortuneDollars(listing.Price))
                return false;

            listing.IsSold = true;

            try
            {
                // Update inventory
                if (listing.Type == NFTType.Character)
                {
                    var nft = (CharacterNFT)listing.NftData;
                    if (!playerInventory.OwnedCharacterIds.Contains(nft.Id))
                    {
                        playerInventory.OwnedCharacterIds.Add(nft.Id);
                        Debug.Log($"Purchased Character NFT: {nft.Name} for {listing.Price} FD");
                    }
                }
                else if (listing.Type == NFTType.Zone)
                {
                    var nft = (ZoneNFT)listing.NftData;
                    if (!playerInventory.OwnedZoneIds.Contains(nft.Id))
                    {
                        playerInventory.OwnedZoneIds.Add(nft.Id);
                        Debug.Log($"Purchased Zone NFT: {nft.Name} for {listing.Price} FD");
                    }
                }

                // Log the updated state after purchase
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.ExportGameState();
                }

                OnBalanceChanged?.Invoke(PlayerProgress.Instance.GetFortuneDollars());
                OnListingsUpdated?.Invoke(GetAvailableListings());

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error purchasing NFT: {e.Message}");
                return false;
            }
        }

        public float GetPlayerBalance()
        {
            return PlayerProgress.Instance.GetFortuneDollars();
        }

        public CharacterNFT GetCharacterNFTById(string id)
        {
            foreach (var listing in listings)
            {
                if (listing.Type == NFTType.Character && listing.Id == id)
                {
                    return (CharacterNFT)listing.NftData;
                }
            }
            return null;
        }

        public List<NFTDisplayData> GetAllListings()
        {
            Debug.Log($"Getting all listings: {listings.Count} items");
            foreach (var listing in listings)
            {
                if (listing.Type == NFTType.Character)
                {
                    var nft = (CharacterNFT)listing.NftData;
                    Debug.Log($"Character NFT: {nft.Name}, Icon: {nft.Icon?.name ?? "null"}, Sprite valid: {nft.Icon != null}, IsSold: {listing.IsSold}");
                }
                else
                {
                    var nft = (ZoneNFT)listing.NftData;
                    Debug.Log($"Zone NFT: {nft.Name}, Icon: {nft.Icon?.name ?? "null"}, Sprite valid: {nft.Icon != null}, IsSold: {listing.IsSold}");
                }
            }
            return listings;
        }

        public MarketplaceState GetCurrentState()
        {
            var state = new MarketplaceState();
            state.PlayerInventory = playerInventory;
            return state;
        }
    }
} 