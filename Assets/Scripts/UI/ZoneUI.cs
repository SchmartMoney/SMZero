using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZoneUI : MonoBehaviour
{
    [System.Serializable]
    private class ZoneSlotUI
    {
        public GameObject slotObject;
        public Image slotImage;
        public Image nftIcon;
        public Button stakeButton;
        public Button unstakeButton;
        public bool isActive;
    }

    [Header("Auto Setup")]
    [SerializeField] private Transform slotsParent; // Parent object containing all zone slots
    [SerializeField] private bool autoSetupFromParent = true;

    [Header("Slot Settings")]
    [SerializeField] private ZoneSlotUI[] slots = new ZoneSlotUI[10];
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private Color activeColor = Color.white;

    [Header("UI References")]
    [SerializeField] private Button returnToMapButton;
    [SerializeField] private TextMeshProUGUI zoneNameText;

    private void Awake()
    {
        if (autoSetupFromParent)
        {
            if (slotsParent == null)
            {
                Debug.LogError("Slots Parent is not assigned! Please assign it in the inspector or disable autoSetupFromParent.");
                return;
            }
            SetupSlotsFromParent();
        }
        else if (slots == null || slots.Length == 0)
        {
            Debug.LogError("Slots array is not configured! Either enable autoSetupFromParent or configure slots manually.");
            slots = new ZoneSlotUI[0];
        }
    }

    private void SetupSlotsFromParent()
    {
        try
        {
            // Resize slots array to match children count
            int childCount = slotsParent.childCount;
            slots = new ZoneSlotUI[childCount];

            // Setup each slot
            for (int i = 0; i < childCount; i++)
            {
                Transform slotTransform = slotsParent.GetChild(i);
                if (slotTransform == null)
                {
                    Debug.LogError($"Child {i} is null in Slots Parent!");
                    continue;
                }

                slots[i] = new ZoneSlotUI
                {
                    slotObject = slotTransform.gameObject,
                    slotImage = slotTransform.GetComponent<Image>(),
                    nftIcon = slotTransform.Find("NFTIcon")?.GetComponent<Image>(),
                    stakeButton = slotTransform.Find("StakeButton")?.GetComponent<Button>(),
                    unstakeButton = slotTransform.Find("UnstakeButton")?.GetComponent<Button>(),
                    isActive = false
                };

                // Validate components
                if (slots[i].slotImage == null)
                    Debug.LogError($"Zone slot {i} ({slotTransform.name}) is missing Image component!");
                if (slots[i].nftIcon == null)
                    Debug.LogError($"Zone slot {i} ({slotTransform.name}) is missing NFTIcon Image!");
                if (slots[i].stakeButton == null)
                    Debug.LogError($"Zone slot {i} ({slotTransform.name}) is missing StakeButton!");
                if (slots[i].unstakeButton == null)
                    Debug.LogError($"Zone slot {i} ({slotTransform.name}) is missing UnstakeButton!");
            }

            Debug.Log($"Auto-setup completed for {childCount} zone slots");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during slot setup: {e.Message}");
            slots = new ZoneSlotUI[0]; // Initialize empty array to prevent null reference
        }
    }

    private void Start()
    {
        // Ensure ZoneManager exists
        if (ZoneManager.Instance == null)
        {
            Debug.LogError("ZoneManager.Instance is null! Creating ZoneManager...");
            var managerObject = new GameObject("ZoneManager");
            var manager = managerObject.AddComponent<ZoneManager>();
            
            if (ZoneManager.Instance == null)
            {
                Debug.LogError("Failed to create ZoneManager! UI functionality will be limited.");
                return;
            }
        }

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("No slots configured. UI functionality will be limited.");
            return;
        }

        // Initialize slots
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null || slots[i].slotObject == null) continue;
            
            bool isActive = i == 0; // Only first slot is active initially
            SetSlotActive(i, isActive);
            SetupSlotButtons(i);
        }

        // Setup return button
        if (returnToMapButton != null)
        {
            returnToMapButton.onClick.AddListener(() => {
                if (ZoneManager.Instance != null)
                {
                    ZoneManager.Instance.ReturnToMap();
                }
                else
                {
                    Debug.LogError("Cannot return to map - ZoneManager is null!");
                }
            });
        }
        else
        {
            Debug.LogWarning("Return to map button is not assigned!");
        }
    }

    private void SetupSlotButtons(int index)
    {
        if (index < 0 || index >= slots.Length || slots[index] == null) return;
        var slot = slots[index];

        if (slot.stakeButton != null)
        {
            slot.stakeButton.onClick.RemoveAllListeners();
            slot.stakeButton.onClick.AddListener(() => OnStakeButtonClicked(index));
        }

        if (slot.unstakeButton != null)
        {
            slot.unstakeButton.onClick.RemoveAllListeners();
            slot.unstakeButton.onClick.AddListener(() => OnUnstakeButtonClicked(index));
        }
    }

    private void OnStakeButtonClicked(int slotIndex)
    {
        if (slots == null || slotIndex < 0 || slotIndex >= slots.Length || !slots[slotIndex].isActive)
        {
            Debug.LogError($"Invalid slot index {slotIndex} or slot is not active");
            return;
        }

        // Check if ZoneManager exists
        if (ZoneManager.Instance == null)
        {
            Debug.LogError("ZoneManager.Instance is null! Creating ZoneManager...");
            // Try to find existing ZoneManager
            var existingManager = FindObjectOfType<ZoneManager>();
            if (existingManager != null)
            {
                Debug.Log("Found existing ZoneManager");
            }
            else
            {
                // Create new ZoneManager if none exists
                var managerObject = new GameObject("ZoneManager");
                var manager = managerObject.AddComponent<ZoneManager>();
                Debug.Log("Created new ZoneManager");
            }

            // If still null after creation attempt, abort
            if (ZoneManager.Instance == null)
            {
                Debug.LogError("Failed to create ZoneManager!");
                return;
            }
        }
        
        // For testing, create a dummy Zone NFT
        var testNFT = new ZoneNFT
        {
            Id = "test_zone_1",
            Name = "Test Zone NFT",
            Icon = null // You'll need to assign a proper icon
        };

        Debug.Log($"Attempting to stake NFT in slot {slotIndex}");
        ZoneManager.Instance.StakeZoneNFT(testNFT);
    }

    private void OnUnstakeButtonClicked(int slotIndex)
    {
        if (slots == null || slotIndex < 0 || slotIndex >= slots.Length || !slots[slotIndex].isActive)
        {
            Debug.LogError($"Invalid slot index {slotIndex} or slot is not active");
            return;
        }

        // Check if ZoneManager exists
        if (ZoneManager.Instance == null)
        {
            Debug.LogError("ZoneManager.Instance is null! Cannot unstake.");
            return;
        }

        Debug.Log($"Attempting to unstake NFT from slot {slotIndex}");
        ZoneManager.Instance.UnstakeZoneNFT();
    }

    public void UpdateZoneSlot(int slotIndex, ZoneNFT nft)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        var slot = slots[slotIndex];
        if (!slot.isActive) return;

        // Update slot visuals
        if (nft != null)
        {
            slot.nftIcon.sprite = nft.Icon;
            slot.nftIcon.gameObject.SetActive(true);
            slot.stakeButton.gameObject.SetActive(false);
            slot.unstakeButton.gameObject.SetActive(true);
        }
        else
        {
            slot.nftIcon.sprite = null;
            slot.nftIcon.gameObject.SetActive(false);
            slot.stakeButton.gameObject.SetActive(true);
            slot.unstakeButton.gameObject.SetActive(false);
        }
    }

    private void SetSlotActive(int index, bool active)
    {
        if (index < 0 || index >= slots.Length) return;

        var slot = slots[index];
        slot.isActive = active;
        slot.slotImage.color = active ? activeColor : inactiveColor;
        slot.stakeButton.gameObject.SetActive(active && slot.nftIcon.sprite == null);
        slot.unstakeButton.gameObject.SetActive(false);
    }

    public void SetZoneName(string zoneName)
    {
        if (zoneNameText != null)
        {
            zoneNameText.text = zoneName;
        }
    }
} 