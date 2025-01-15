using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Linq;

namespace SMZero
{
    public class MarketplaceManager : MonoBehaviour
    {
        private static MarketplaceManager instance;
        public static MarketplaceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<MarketplaceManager>();
                    if (instance == null)
                    {
                        Debug.Log("Creating new MarketplaceManager instance");
                        GameObject go = new GameObject("MarketplaceManager");
                        instance = go.AddComponent<MarketplaceManager>();
                        
                        // Load sprites when creating instance dynamically
                        instance.LoadSprites();
                    }
                }
                return instance;
            }
        }

        [Header("NFTs")]
        private List<CharacterNFT> characterNFTs = new List<CharacterNFT>();
        private List<ZoneNFT> zoneNFTs = new List<ZoneNFT>();
        private MarketplaceState currentState;

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

        private bool hasInitialized = false;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (!hasInitialized)
            {
                InitializeMarketplace();
                hasInitialized = true;
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
            
            // Load sprites first
            LoadSprites();
            ValidateSprites();
            
            // Only clear and reinitialize NFTs if they're empty
            if (characterNFTs.Count == 0 && zoneNFTs.Count == 0)
            {
                Debug.Log("First time initialization - Creating NFTs...");
                
                // Add Character NFTs
                AddCharacterNFT("Richard", richardSprite, NFTRarity.Rare, 400f, 1.0f, 1.0f);
                AddCharacterNFT("Emily", emilySprite, NFTRarity.Epic, 6000f, 0.85f, 1.0f);
                AddCharacterNFT("Jake", jakeSprite, NFTRarity.Legendary, 12000f, 0.85f, 2.0f);
                
                // Add Zone NFTs
                AddZoneNFT("Vault Avenue", vaultAvenueSprite, NFTRarity.Rare, 500f);
            }
            else
            {
                Debug.Log("NFTs already initialized - Preserving existing NFTs");
                Debug.Log($"Existing NFTs: {characterNFTs.Count} characters, {zoneNFTs.Count} zones");
            }
            
            // Only initialize state if it doesn't exist
            if (currentState == null)
            {
                Debug.Log("Creating new marketplace state");
                currentState = new MarketplaceState
                {
                    PlayerInventory = playerInventory,
                    PlayerBalance = PlayerProgress.Instance?.GetFortuneDollars() ?? 1000f
                };
            }
            
            // Always update listings based on current state
            listings.Clear(); // Clear existing listings
            
            // Recreate listings from NFTs
            foreach (var nft in characterNFTs)
            {
                var displayData = new NFTDisplayData
                {
                    Id = nft.Id,
                    Type = NFTType.Character,
                    NftData = nft,
                    Price = nft.cost,
                    IsSold = currentState.PlayerInventory.OwnedCharacterIds?.Contains(nft.Id) ?? false
                };
                listings.Add(displayData);
            }
            
            foreach (var nft in zoneNFTs)
            {
                var displayData = new NFTDisplayData
                {
                    Id = nft.Id,
                    Type = NFTType.Zone,
                    NftData = nft,
                    Price = nft.cost,
                    IsSold = currentState.PlayerInventory.OwnedZoneIds?.Contains(nft.Id) ?? false
                };
                listings.Add(displayData);
            }
            
            // Log marketplace status
            Debug.Log("=== Marketplace Status ===");
            Debug.Log($"- {characterNFTs.Count} character NFTs");
            Debug.Log($"- {zoneNFTs.Count} zone NFTs");
            Debug.Log($"- {listings.Count} total listings");
            Debug.Log($"- Current inventory: {currentState.PlayerInventory.OwnedCharacterIds?.Count ?? 0} characters, {currentState.PlayerInventory.OwnedZoneIds?.Count ?? 0} zones");
            Debug.Log($"- Owned zone IDs: {string.Join(", ", currentState.PlayerInventory.OwnedZoneIds ?? new List<string>())}");
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

            // Check if this NFT already exists
            if (characterNFTs.Any(c => c.Id == id))
            {
                Debug.Log($"Character NFT {name} (ID: {id}) already exists, skipping creation");
                return;
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

            // Only add to listings if it doesn't exist
            if (!listings.Any(l => l.Id == id))
            {
                var displayData = new NFTDisplayData
                {
                    Id = nft.Id,
                    Type = NFTType.Character,
                    NftData = nft,
                    Price = price,
                    IsSold = playerInventory.OwnedCharacterIds.Contains(nft.Id)
                };
                listings.Add(displayData);
            }

            characterNFTs.Add(nft);

            // Verify the NFT data after creation
            Debug.Log($"Added Character NFT: {name}, ID: {id}, Price: {price}, Icon: {icon.name}, Sprite valid: {icon != null}, IsSold: {playerInventory.OwnedCharacterIds.Contains(id)}");
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

            // Check if this NFT already exists
            if (zoneNFTs.Any(z => z.Id == id))
            {
                Debug.Log($"Zone NFT {name} (ID: {id}) already exists, skipping creation");
                return;
            }

            var nft = new ZoneNFT
            {
                Id = id,
                Name = name,
                Icon = icon,
                Rarity = rarity,
                cost = price
            };

            // Only add to listings if it doesn't exist
            if (!listings.Any(l => l.Id == id))
            {
                var displayData = new NFTDisplayData
                {
                    Id = nft.Id,
                    Type = NFTType.Zone,
                    NftData = nft,
                    Price = price,
                    IsSold = playerInventory.OwnedZoneIds.Contains(nft.Id)
                };
                listings.Add(displayData);
            }

            zoneNFTs.Add(nft);

            // Verify the NFT data after creation
            Debug.Log($"Added Zone NFT: {name}, ID: {id}, Price: {price}, Icon: {icon.name}, Sprite valid: {icon != null}, IsSold: {playerInventory.OwnedZoneIds.Contains(id)}");
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

        public List<ZoneNFT> GetOwnedZoneNFTs()
        {
            Debug.Log("=== GetOwnedZoneNFTs called ===");
            Debug.Log($"currentState null? {currentState == null}");
            Debug.Log($"playerInventory null? {playerInventory == null}");
            
            if (currentState?.PlayerInventory?.OwnedZoneIds == null)
            {
                Debug.Log("No owned zone IDs found in state");
                return new List<ZoneNFT>();
            }

            Debug.Log($"Total zone NFTs available: {zoneNFTs.Count}");
            Debug.Log($"Owned zone IDs in state: {string.Join(", ", currentState.PlayerInventory.OwnedZoneIds)}");
            
            var ownedZones = zoneNFTs.Where(z => currentState.PlayerInventory.OwnedZoneIds.Contains(z.Id)).ToList();
            Debug.Log($"Found {ownedZones.Count} owned zones");
            foreach (var zone in ownedZones)
            {
                Debug.Log($"- Owned zone: {zone.Name} (ID: {zone.Id})");
            }
            
            return ownedZones;
        }

        public bool OwnsZoneNFT(string zoneId)
        {
            return currentState?.PlayerInventory?.OwnedZoneIds?.Contains(zoneId) ?? false;
        }

        public ZoneNFT GetZoneNFTById(string zoneId)
        {
            return zoneNFTs.FirstOrDefault(z => z.Id == zoneId);
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
            Debug.Log("=== Restoring marketplace state ===");
            if (state == null)
            {
                Debug.LogWarning("Attempted to restore null marketplace state, initializing new state");
                playerInventory = new PlayerInventory();
                currentState = new MarketplaceState
                {
                    PlayerInventory = playerInventory,
                    PlayerBalance = PlayerProgress.Instance?.GetFortuneDollars() ?? 1000f
                };
                return;
            }

            Debug.Log($"Restoring state with {state.PlayerInventory?.OwnedCharacterIds?.Count ?? 0} characters and {state.PlayerInventory?.OwnedZoneIds?.Count ?? 0} zones");
            
            // Restore inventory and state
            playerInventory = state.PlayerInventory ?? new PlayerInventory();
            currentState = state;

            // Force reinitialize NFTs
            Debug.Log("Reinitializing NFTs during state restore...");
            characterNFTs.Clear();
            zoneNFTs.Clear();
            
            // Add Character NFTs
            AddCharacterNFT("Richard", richardSprite, NFTRarity.Rare, 400f, 1.0f, 1.0f);
            AddCharacterNFT("Emily", emilySprite, NFTRarity.Epic, 6000f, 0.85f, 1.0f);
            AddCharacterNFT("Jake", jakeSprite, NFTRarity.Legendary, 12000f, 0.85f, 2.0f);
            
            // Add Zone NFTs
            AddZoneNFT("Vault Avenue", vaultAvenueSprite, NFTRarity.Rare, 500f);

            // Update sold status of listings based on inventory
            foreach (var listing in listings)
            {
                if (listing?.NftData == null) continue;

                if (listing.Type == NFTType.Character)
                {
                    var nft = (CharacterNFT)listing.NftData;
                    bool wasOwned = listing.IsSold;
                    listing.IsSold = playerInventory.OwnedCharacterIds?.Contains(nft.Id) ?? false;
                    Debug.Log($"Character {nft.Name} ({nft.Id}): Was owned: {wasOwned}, Is owned: {listing.IsSold}");
                }
                else if (listing.Type == NFTType.Zone)
                {
                    var nft = (ZoneNFT)listing.NftData;
                    bool wasOwned = listing.IsSold;
                    listing.IsSold = playerInventory.OwnedZoneIds?.Contains(nft.Id) ?? false;
                    Debug.Log($"Zone {nft.Name} ({nft.Id}): Was owned: {wasOwned}, Is owned: {listing.IsSold}");
                }
            }

            Debug.Log("=== NFT Status After Restore ===");
            Debug.Log($"Total Character NFTs: {characterNFTs.Count}");
            Debug.Log($"Total Zone NFTs: {zoneNFTs.Count}");
            Debug.Log($"Owned Zone IDs: {string.Join(", ", playerInventory.OwnedZoneIds)}");
            var ownedZones = GetOwnedZoneNFTs();
            Debug.Log($"Owned Zones Count: {ownedZones.Count}");
            foreach (var zone in ownedZones)
            {
                Debug.Log($"- Owned Zone: {zone.Name} (ID: {zone.Id})");
            }

            // Notify UI
            OnListingsUpdated?.Invoke(GetAvailableListings());
            if (PlayerProgress.Instance != null)
            {
                OnBalanceChanged?.Invoke(PlayerProgress.Instance.GetFortuneDollars());
            }
            
            Debug.Log($"=== State restored ===");
            Debug.Log($"- Listings: {listings.Count}");
            Debug.Log($"- Player inventory: {playerInventory.OwnedCharacterIds.Count} characters, {playerInventory.OwnedZoneIds.Count} zones");
            Debug.Log($"- Owned zone IDs: {string.Join(", ", playerInventory.OwnedZoneIds)}");
        }

        public bool TryPurchaseNFT(NFTDisplayData listing)
        {
            Debug.Log("=== Attempting to purchase NFT ===");
            if (listing == null || listing.NftData == null || PlayerProgress.Instance == null)
            {
                Debug.LogError("Cannot purchase NFT: null reference detected");
                return false;
            }

            Debug.Log($"Attempting to purchase {(listing.Type == NFTType.Character ? "Character" : "Zone")} NFT");
            Debug.Log($"- Is sold? {listing.IsSold}");
            Debug.Log($"- Current balance: {PlayerProgress.Instance.GetFortuneDollars()}");
            Debug.Log($"- Price: {listing.Price}");

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
                        Debug.Log($"Purchased Character NFT: {nft.Name} (ID: {nft.Id}) for {listing.Price} FD");
                    }
                }
                else if (listing.Type == NFTType.Zone)
                {
                    var nft = (ZoneNFT)listing.NftData;
                    if (!playerInventory.OwnedZoneIds.Contains(nft.Id))
                    {
                        playerInventory.OwnedZoneIds.Add(nft.Id);
                        Debug.Log($"Purchased Zone NFT: {nft.Name} (ID: {nft.Id}) for {listing.Price} FD");
                    }
                }

                // Update current state
                if (currentState == null)
                {
                    Debug.Log("Creating new marketplace state");
                    currentState = new MarketplaceState();
                }
                currentState.PlayerInventory = playerInventory;
                currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();

                Debug.Log("=== Updated State ===");
                Debug.Log($"- Balance: {currentState.PlayerBalance}");
                Debug.Log($"- Owned Characters: {string.Join(", ", currentState.PlayerInventory.OwnedCharacterIds)}");
                Debug.Log($"- Owned Zones: {string.Join(", ", currentState.PlayerInventory.OwnedZoneIds)}");

                // Log the updated state after purchase
                if (GameStateManager.Instance != null)
                {
                    Debug.Log("Exporting game state after purchase");
                    GameStateManager.Instance.ExportGameState();
                }

                OnBalanceChanged?.Invoke(PlayerProgress.Instance.GetFortuneDollars());
                OnListingsUpdated?.Invoke(GetAvailableListings());

                Debug.Log($"Purchase successful. Updated inventory: {playerInventory.OwnedCharacterIds.Count} characters, {playerInventory.OwnedZoneIds.Count} zones");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error purchasing NFT: {e.Message}\n{e.StackTrace}");
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
            Debug.Log("Current listings:");
            foreach (var listing in listings)
            {
                if (listing.Type == NFTType.Character)
                {
                    var nft = (CharacterNFT)listing.NftData;
                    Debug.Log($"- Character: {nft.Name} ({nft.Rarity}), ID: {nft.Id}, Price: {listing.Price} FD, Sold: {listing.IsSold}");
                }
                else
                {
                    var nft = (ZoneNFT)listing.NftData;
                    Debug.Log($"- Zone: {nft.Name} ({nft.Rarity}), ID: {nft.Id}, Price: {listing.Price} FD, Sold: {listing.IsSold}");
                }
            }
            return listings;
        }

        public MarketplaceState GetCurrentState()
        {
            if (currentState == null)
            {
                currentState = new MarketplaceState
                {
                    PlayerInventory = playerInventory,
                    PlayerBalance = PlayerProgress.Instance?.GetFortuneDollars() ?? 1000f
                };
            }
            else
            {
                // Update balance in current state
                if (PlayerProgress.Instance != null)
                {
                    currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();
                }
            }
            return currentState;
        }

        public List<CharacterNFT> GetOwnedCharacterNFTs()
        {
            if (currentState?.PlayerInventory?.OwnedCharacterIds == null)
                return new List<CharacterNFT>();

            return characterNFTs.Where(c => currentState.PlayerInventory.OwnedCharacterIds.Contains(c.Id)).ToList();
        }

        private void LoadSprites()
        {
            Debug.Log("Loading NFT sprites...");
            
            // Load character sprites
            richardSprite = Resources.Load<Sprite>("NFTs/Characters/Richard");
            emilySprite = Resources.Load<Sprite>("NFTs/Characters/Emily");
            jakeSprite = Resources.Load<Sprite>("NFTs/Characters/Jake");
            
            // Load zone sprites
            vaultAvenueSprite = Resources.Load<Sprite>("NFTs/Zones/VaultAvenue");
            
            // Log results
            Debug.Log($"Loaded sprites - Richard: {(richardSprite != null ? "Success" : "Failed")}, " +
                     $"Emily: {(emilySprite != null ? "Success" : "Failed")}, " +
                     $"Jake: {(jakeSprite != null ? "Success" : "Failed")}, " +
                     $"VaultAve: {(vaultAvenueSprite != null ? "Success" : "Failed")}");
        }
    }
} 