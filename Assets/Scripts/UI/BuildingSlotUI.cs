using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace SMZero
{
    public class BuildingSlotUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button stakeButton;
        [SerializeField] private Button unstakeButton;
        [SerializeField] private TextMeshProUGUI nftText;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool isActive;
        private bool isFirstSlot;
        private bool hasInitialized = false;

        private void Awake()
        {
            if (hasInitialized)
            {
                Debug.LogWarning($"[{gameObject.name}] Already initialized, destroying duplicate!");
                Destroy(gameObject);
                return;
            }
            
            ValidateReferences();
            SetupButtons();
            hasInitialized = true;
        }

        private void ValidateReferences()
        {
            Debug.Log($"[{gameObject.name}] Validating references...");
            if (canvasGroup == null) Debug.LogError($"[{gameObject.name}] Canvas group is missing!");
            
            // Only the exact name "ZoneSlot_0" is considered the first slot
            isFirstSlot = gameObject.name == "ZoneSlot_0";
            Debug.Log($"[{gameObject.name}] Is first slot? {isFirstSlot}");
            
            if (isFirstSlot)
            {
                if (stakeButton == null) Debug.LogError($"[{gameObject.name}] Stake button is missing!");
                if (unstakeButton == null) Debug.LogError($"[{gameObject.name}] Unstake button is missing!");
                Debug.Log($"[{gameObject.name}] Stake button null? {stakeButton == null}, Unstake button null? {unstakeButton == null}");
            }
            else
            {
                // Hide stake/unstake buttons for non-first slots
                if (stakeButton != null) stakeButton.gameObject.SetActive(false);
                if (unstakeButton != null) unstakeButton.gameObject.SetActive(false);
            }
        }

        private void SetupButtons()
        {
            Debug.Log($"[{gameObject.name}] Setting up buttons...");
            if (isFirstSlot)
            {
                if (stakeButton != null)
                {
                    Debug.Log($"[{gameObject.name}] Setting up stake button");
                    stakeButton.onClick.RemoveAllListeners();
                    stakeButton.onClick.AddListener(OnStakeClicked);
                    stakeButton.interactable = true;
                    Debug.Log($"[{gameObject.name}] Stake button interactable: {stakeButton.interactable}");
                }

                if (unstakeButton != null)
                {
                    Debug.Log($"[{gameObject.name}] Setting up unstake button");
                    unstakeButton.onClick.RemoveAllListeners();
                    unstakeButton.onClick.AddListener(OnUnstakeClicked);
                    unstakeButton.interactable = true;
                    Debug.Log($"[{gameObject.name}] Unstake button interactable: {unstakeButton.interactable}");
                }
            }

            UpdateButtonsVisibility();
        }

        public void Initialize(bool isEnabled)
        {
            if (!hasInitialized)
            {
                Debug.LogError($"[{gameObject.name}] Initialize called before Awake!");
                return;
            }

            Debug.Log($"[{gameObject.name}] Initializing with enabled: {isEnabled}");
            isActive = isEnabled;
            
            // Update canvas group
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f; // Always show the slot
                canvasGroup.interactable = isFirstSlot; // Only first slot is interactable
                canvasGroup.blocksRaycasts = true;
                Debug.Log($"[{gameObject.name}] Canvas group updated - alpha: {canvasGroup.alpha}, interactable: {canvasGroup.interactable}");
            }

            // Update buttons visibility based on NFT ownership and staking status
            if (isFirstSlot)
            {
                var marketplaceManager = MarketplaceManager.Instance;
                var zoneManager = ZoneManager.Instance;
                
                if (marketplaceManager != null && zoneManager != null)
                {
                    var ownedZones = marketplaceManager.GetOwnedZoneNFTs();
                    var hasVaultAvenueNFT = ownedZones?.Any(z => z.Id == "vault-avenue-001") ?? false;
                    var isStaked = zoneManager.GetActiveZoneNFT() != null;
                    
                    Debug.Log($"[{gameObject.name}] Has Vault Avenue NFT? {hasVaultAvenueNFT}, Is Staked? {isStaked}");
                    
                    if (stakeButton != null)
                    {
                        stakeButton.gameObject.SetActive(hasVaultAvenueNFT && !isStaked);
                        stakeButton.interactable = hasVaultAvenueNFT && !isStaked;
                        Debug.Log($"[{gameObject.name}] Stake button active: {hasVaultAvenueNFT && !isStaked}");
                    }
                    
                    if (unstakeButton != null)
                    {
                        unstakeButton.gameObject.SetActive(hasVaultAvenueNFT && isStaked);
                        unstakeButton.interactable = hasVaultAvenueNFT && isStaked;
                        Debug.Log($"[{gameObject.name}] Unstake button active: {hasVaultAvenueNFT && isStaked}");
                    }
                }
            }
        }

        public void SetNFTText(string text)
        {
            if (nftText != null)
            {
                nftText.text = text;
            }
        }

        private void UpdateButtonsVisibility()
        {
            if (!isActive || !isFirstSlot) return;

            Debug.Log("Updating buttons visibility...");
            var zoneManager = ZoneManager.Instance;
            if (zoneManager != null)
            {
                var activeZoneNFT = zoneManager.GetActiveZoneNFT();
                bool hasStakedNFT = activeZoneNFT != null;
                Debug.Log($"Has staked NFT? {hasStakedNFT}");

                if (stakeButton != null)
                {
                    stakeButton.gameObject.SetActive(!hasStakedNFT);
                    stakeButton.interactable = !hasStakedNFT;
                    Debug.Log($"Stake button active? {!hasStakedNFT}");
                }

                if (unstakeButton != null)
                {
                    unstakeButton.gameObject.SetActive(hasStakedNFT);
                    unstakeButton.interactable = hasStakedNFT;
                    Debug.Log($"Unstake button active? {hasStakedNFT}");
                }
            }
            else
            {
                Debug.LogError("ZoneManager not found!");
            }
        }

        private void OnStakeClicked()
        {
            if (!isFirstSlot) return;
            
            Debug.Log($"[{gameObject.name}] Stake button clicked");
            var marketplaceManager = MarketplaceManager.Instance;
            if (marketplaceManager != null)
            {
                var ownedZones = marketplaceManager.GetOwnedZoneNFTs();
                Debug.Log($"Found {ownedZones?.Count ?? 0} owned zones");
                
                if (ownedZones != null && ownedZones.Count > 0)
                {
                    var vaultAvenueNFT = ownedZones.Find(z => z.Id == "vault-avenue-001");
                    if (vaultAvenueNFT != null)
                    {
                        Debug.Log("Found Vault Avenue NFT, attempting to stake");
                        var zoneManager = ZoneManager.Instance;
                        if (zoneManager != null)
                        {
                            zoneManager.StakeZoneNFT(vaultAvenueNFT);
                            UpdateButtonsVisibility();
                        }
                        else
                        {
                            Debug.LogError("ZoneManager not found!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Vault Avenue NFT not found in owned zones!");
                    }
                }
                else
                {
                    Debug.LogWarning("No owned Zone NFTs found!");
                }
            }
            else
            {
                Debug.LogError("MarketplaceManager not found!");
            }
        }

        private void OnUnstakeClicked()
        {
            if (!isFirstSlot) return;
            
            Debug.Log($"[{gameObject.name}] Unstake button clicked");
            ZoneManager.Instance?.UnstakeZoneNFT();
            UpdateButtonsVisibility();
            
            // Keep the UI active and update the state
            isActive = true;
            SetupButtons();
        }

        private void OnDestroy()
        {
            if (stakeButton != null)
            {
                stakeButton.onClick.RemoveAllListeners();
            }

            if (unstakeButton != null)
            {
                unstakeButton.onClick.RemoveAllListeners();
            }
        }
    }
} 