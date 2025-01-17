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
                    instance = GameManagers.Instance.Marketplace;
                }
                return instance;
            }
        }

        [Header("NFTs")]
        private List<CharacterNFT> characterNFTs = new List<CharacterNFT>();
        private List<ZoneNFT> zoneNFTs = new List<ZoneNFT>();
        private MarketplaceState currentState;
        private List<NFTDisplayData> listings = new List<NFTDisplayData>();
        private PlayerInventory playerInventory = new PlayerInventory();

        [Header("Character NFTs")]
        private Sprite richardSprite;
        private Sprite emilySprite;
        private Sprite jakeSprite;

        [Header("Zone NFTs")]
        private Sprite vaultAvenueSprite;

        public UnityEvent<float> OnBalanceChanged = new UnityEvent<float>();
        public UnityEvent<List<NFTDisplayData>> OnListingsUpdated = new UnityEvent<List<NFTDisplayData>>();

        private void Awake()
        {
            Debug.Log("[MarketplaceManager] Awake called");
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            LoadSprites();
        }

        private void LoadSprites()
        {
            Debug.Log("[MarketplaceManager] Loading sprites");
            richardSprite = Resources.Load<Sprite>("NFTs/Characters/Richard");
            emilySprite = Resources.Load<Sprite>("NFTs/Characters/Emily");
            jakeSprite = Resources.Load<Sprite>("NFTs/Characters/Jake");
            vaultAvenueSprite = Resources.Load<Sprite>("NFTs/Zones/VaultAvenue");
            ValidateSprites();
        }

        private void ValidateSprites()
        {
            if (richardSprite == null) Debug.LogError("[MarketplaceManager] Richard sprite not found");
            if (emilySprite == null) Debug.LogError("[MarketplaceManager] Emily sprite not found");
            if (jakeSprite == null) Debug.LogError("[MarketplaceManager] Jake sprite not found");
            if (vaultAvenueSprite == null) Debug.LogError("[MarketplaceManager] Vault Avenue sprite not found");
        }

        private void InitializeMarketplace()
        {
            Debug.Log("[MarketplaceManager] Initializing marketplace");
            
            // Only initialize NFTs if they're empty
            if (characterNFTs.Count == 0 && zoneNFTs.Count == 0)
            {
                // Add Character NFTs
                AddCharacterNFT("Richard", richardSprite, NFTRarity.Common, 400f, 1.0f, 1.0f);
                AddCharacterNFT("Emily", emilySprite, NFTRarity.Rare, 6000f, 0.85f, 1.0f);
                AddCharacterNFT("Jake", jakeSprite, NFTRarity.Epic, 12000f, 0.85f, 2.0f);
                
                // Add Zone NFTs
                AddZoneNFT("Vault Avenue", vaultAvenueSprite, NFTRarity.Common, 500f);
            }
            
            // Initialize state if it doesn't exist
            if (currentState == null)
            {
                currentState = new MarketplaceState
                {
                    PlayerInventory = playerInventory,
                    PlayerBalance = PlayerProgress.Instance?.GetFortuneDollars() ?? 1000f
                };
            }
            
            Debug.Log("[MarketplaceManager] Marketplace initialized");
        }

        private void Start()
        {
            Debug.Log("[MarketplaceManager] Start called");
            InitializeMarketplace();
            
            // Wait for GameState to be initialized
            var gameState = GameStateManager.Instance;
            if (gameState == null)
            {
                Debug.LogError("[MarketplaceManager] GameStateManager instance not found");
                return;
            }

            if (!gameState.IsInitialized)
            {
                Debug.LogWarning("[MarketplaceManager] GameState not yet initialized");
                return;
            }

            // Get current state from GameState
            var state = gameState.GetCurrentState();
            if (state?.PlayerInventory != null)
            {
                Debug.Log("[MarketplaceManager] Restoring state from GameState");
                RestoreState(new MarketplaceState { PlayerInventory = state.PlayerInventory });
            }
            else
            {
                Debug.LogWarning("[MarketplaceManager] No valid state found in GameState");
            }
        }

        private void Update()
        {
            // Skip if GameStateManager is not ready
            if (GameStateManager.Instance == null || !GameStateManager.Instance.IsInitialized) 
            {
                return;
            }
            
            var state = GameStateManager.Instance.GetCurrentState();
            if (state?.PlayerInventory == null) return;
            
            bool stateChanged = false;

            // Update character ownership
            foreach (var listing in listings.Where(l => l.Type == NFTType.Character))
            {
                var nft = (CharacterNFT)listing.NftData;
                bool wasOwned = listing.IsSold;
                listing.IsSold = state.PlayerInventory.OwnedCharacterIds?.Contains(nft.Id) ?? false;
                
                if (wasOwned != listing.IsSold)
                {
                    Debug.Log($"[MarketplaceManager] Character {nft.Id} ownership changed to {listing.IsSold}");
                    stateChanged = true;
                }
            }

            // Update zone ownership
            foreach (var listing in listings.Where(l => l.Type == NFTType.Zone))
            {
                var nft = (ZoneNFT)listing.NftData;
                bool wasOwned = listing.IsSold;
                listing.IsSold = state.PlayerInventory.OwnedZoneIds?.Contains(nft.Id) ?? false;
                nft.isActive = listing.IsSold;
                
                if (wasOwned != listing.IsSold)
                {
                    Debug.Log($"[MarketplaceManager] Zone {nft.Id} ownership changed to {listing.IsSold}");
                    stateChanged = true;
                }
            }

            // Notify listeners if state changed
            if (stateChanged)
            {
                OnListingsUpdated?.Invoke(GetAvailableListings());
            }
        }

        public void RestoreState(MarketplaceState state)
        {
            Debug.Log("[MarketplaceManager] Restoring marketplace state");
            if (state?.PlayerInventory == null)
            {
                Debug.LogWarning("[MarketplaceManager] State or inventory is null, skipping restore");
                return;
            }

            // Update character ownership
            foreach (var listing in listings.Where(l => l.Type == NFTType.Character))
            {
                var nft = (CharacterNFT)listing.NftData;
                listing.IsSold = state.PlayerInventory.OwnedCharacterIds?.Contains(nft.Id) ?? false;
            }
            Debug.Log($"[MarketplaceManager] Restored {listings.Count(l => l.Type == NFTType.Character && l.IsSold)} owned characters");

            // Update zone ownership
            foreach (var listing in listings.Where(l => l.Type == NFTType.Zone))
            {
                var nft = (ZoneNFT)listing.NftData;
                listing.IsSold = state.PlayerInventory.OwnedZoneIds?.Contains(nft.Id) ?? false;
                nft.isActive = listing.IsSold;
            }
            Debug.Log($"[MarketplaceManager] Restored {listings.Count(l => l.Type == NFTType.Zone && l.IsSold)} owned zones");
            
            // Update current state
            currentState = state;
            playerInventory = state.PlayerInventory;
            
            // Notify listeners
            OnListingsUpdated?.Invoke(GetAvailableListings());
            
            Debug.Log("[MarketplaceManager] State restoration complete");
        }

        private void AddCharacterNFT(string name, Sprite icon, NFTRarity rarity, float price, float speedMod, float amountMod)
        {
            if (icon == null)
            {
                Debug.LogError($"[MarketplaceManager] Cannot add Character NFT {name}: icon is null!");
                return;
            }

            // Use consistent IDs based on name
            string id = name.ToLower() switch
            {
                "richard" => "richard-nft-001",
                "emily" => "emily-nft-001",
                "jake" => "jake-nft-001",
                _ => System.Guid.NewGuid().ToString()
            };

            // Check if this NFT already exists
            if (characterNFTs.Any(c => c.Id == id))
            {
                Debug.Log($"[MarketplaceManager] Character NFT {id} already exists");
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

            // Add to listings if it doesn't exist
            if (!listings.Any(l => l.Id == id))
            {
                var displayData = new NFTDisplayData
                {
                    Id = nft.Id,
                    Type = NFTType.Character,
                    NftData = nft,
                    Price = price,
                    IsSold = playerInventory.OwnedCharacterIds?.Contains(nft.Id) ?? false
                };
                listings.Add(displayData);
                Debug.Log($"[MarketplaceManager] Added Character NFT listing: {name}");
            }

            characterNFTs.Add(nft);
            Debug.Log($"[MarketplaceManager] Added Character NFT: {name}");
        }

        private void AddZoneNFT(string name, Sprite icon, NFTRarity rarity, float price)
        {
            if (icon == null)
            {
                Debug.LogError($"[MarketplaceManager] Cannot add Zone NFT {name}: icon is null!");
                return;
            }

            // Use consistent IDs based on name
            string id = name.ToLower() switch
            {
                "vault avenue" => "vault-avenue-001",
                _ => System.Guid.NewGuid().ToString()
            };

            // Check if this NFT already exists
            if (zoneNFTs.Any(z => z.Id == id))
            {
                Debug.Log($"[MarketplaceManager] Zone NFT {id} already exists");
                return;
            }

            var nft = new ZoneNFT
            {
                Id = id,
                Name = name,
                Icon = icon,
                Rarity = rarity,
                cost = price,
                isActive = false
            };

            // Add to listings if it doesn't exist
            if (!listings.Any(l => l.Id == id))
            {
                var displayData = new NFTDisplayData
                {
                    Id = nft.Id,
                    Type = NFTType.Zone,
                    NftData = nft,
                    Price = price,
                    IsSold = playerInventory.OwnedZoneIds?.Contains(nft.Id) ?? false
                };
                listings.Add(displayData);
                Debug.Log($"[MarketplaceManager] Added Zone NFT listing: {name}");
            }

            zoneNFTs.Add(nft);
            Debug.Log($"[MarketplaceManager] Added Zone NFT: {name}");
        }

        public List<CharacterNFT> GetOwnedCharacterNFTs()
        {
            var owned = characterNFTs.Where(nft => listings.Any(l => l.NftData == nft && l.IsSold)).ToList();
            Debug.Log($"[MarketplaceManager] Found {owned.Count} owned characters");
            return owned;
        }

        public List<ZoneNFT> GetOwnedZoneNFTs()
        {
            var owned = zoneNFTs.Where(nft => listings.Any(l => l.NftData == nft && l.IsSold)).ToList();
            Debug.Log($"[MarketplaceManager] Found {owned.Count} owned zones");
            return owned;
        }

        public bool TryPurchaseNFT(NFTDisplayData listing)
        {
            if (listing == null || listing.NftData == null || PlayerProgress.Instance == null)
            {
                Debug.LogError("[MarketplaceManager] Cannot purchase NFT: null reference detected");
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
                        Debug.Log($"[MarketplaceManager] Added Character NFT {nft.Id} to inventory");
                    }
                }
                else if (listing.Type == NFTType.Zone)
                {
                    var nft = (ZoneNFT)listing.NftData;
                    if (!playerInventory.OwnedZoneIds.Contains(nft.Id))
                    {
                        playerInventory.OwnedZoneIds.Add(nft.Id);
                        nft.isActive = true;
                        Debug.Log($"[MarketplaceManager] Added Zone NFT {nft.Id} to inventory");
                    }
                }

                // Update current state
                if (currentState == null)
                {
                    currentState = new MarketplaceState();
                }
                currentState.PlayerInventory = playerInventory;
                currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();

                // Export state
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.ExportGameState();
                }

                OnBalanceChanged?.Invoke(PlayerProgress.Instance.GetFortuneDollars());
                OnListingsUpdated?.Invoke(GetAvailableListings());

                Debug.Log($"[MarketplaceManager] Successfully purchased NFT {listing.Id}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[MarketplaceManager] Error purchasing NFT: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public List<NFTDisplayData> GetAvailableListings()
        {
            return listings.FindAll(l => !l.IsSold);
        }

        public List<NFTDisplayData> GetAllListings()
        {
            return listings;
        }

        public bool OwnsZoneNFT(string zoneId)
        {
            return currentState?.PlayerInventory?.OwnedZoneIds?.Contains(zoneId) ?? false;
        }

        public ZoneNFT GetZoneNFTById(string zoneId)
        {
            return zoneNFTs.FirstOrDefault(z => z.Id == zoneId);
        }

        public CharacterNFT GetCharacterNFTById(string id)
        {
            return characterNFTs.FirstOrDefault(c => c.Id == id);
        }

        public float GetPlayerBalance()
        {
            return PlayerProgress.Instance?.GetFortuneDollars() ?? 0f;
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
            else if (PlayerProgress.Instance != null)
            {
                currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();
            }
            return currentState;
        }
    }
} 