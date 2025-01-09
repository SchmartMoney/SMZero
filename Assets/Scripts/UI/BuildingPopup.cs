using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingPopup : MonoBehaviour
{
    public static BuildingPopup Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button collectButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject buildingModelContainer;

    [Header("NFT Slots Setup")]
    [SerializeField] private Transform nftSlotsParent;
    [SerializeField] private bool autoSetupSlots = true;
    [SerializeField] private CharacterSlotUI[] nftSlots = new CharacterSlotUI[3];

    private Building currentBuilding;
    private Canvas parentCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (autoSetupSlots)
        {
            SetupNFTSlots();
        }

        ValidateComponents();
        SetupUI();
    }

    private void SetupNFTSlots()
    {
        if (nftSlotsParent == null)
        {
            Debug.LogError("NFT Slots Parent is not assigned!");
            return;
        }

        try
        {
            // Resize slots array to match children count
            int childCount = Mathf.Min(nftSlotsParent.childCount, 3); // Maximum 3 slots
            nftSlots = new CharacterSlotUI[childCount];

            // Setup each slot
            for (int i = 0; i < childCount; i++)
            {
                Transform slotTransform = nftSlotsParent.GetChild(i);
                if (slotTransform == null)
                {
                    Debug.LogError($"Child {i} is null in NFT Slots Parent!");
                    continue;
                }

                var slotUI = slotTransform.GetComponent<CharacterSlotUI>();
                if (slotUI == null)
                {
                    Debug.LogError($"NFT slot {i} ({slotTransform.name}) is missing CharacterSlotUI component!");
                    continue;
                }

                nftSlots[i] = slotUI;
            }

            Debug.Log($"Auto-setup completed for {childCount} NFT slots");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during NFT slot setup: {e.Message}");
            nftSlots = new CharacterSlotUI[0];
        }
    }

    private void ValidateComponents()
    {
        if (popupPanel == null) Debug.LogError("PopupPanel not assigned!");
        if (titleText == null) Debug.LogError("TitleText not assigned!");
        if (timerText == null) Debug.LogError("TimerText not assigned!");
        if (infoText == null) Debug.LogError("InfoText not assigned!");
        if (collectButton == null) Debug.LogError("CollectButton not assigned!");
        if (closeButton == null) Debug.LogError("CloseButton not assigned!");
        if (buildingModelContainer == null) Debug.LogError("BuildingModelContainer not assigned!");
        
        // Check NFT slots
        if (nftSlots == null || nftSlots.Length == 0)
        {
            Debug.LogError("NFT slots array not properly configured!");
        }
        else
        {
            for (int i = 0; i < nftSlots.Length; i++)
            {
                if (nftSlots[i] == null) Debug.LogError($"NFT slot {i} not assigned!");
            }
        }

        // Validate parent Canvas
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("BuildingPopup must be child of a Canvas!");
        }
    }

    private void SetupUI()
    {
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => {
                Debug.Log("Close button clicked");
                Hide();
            });
        }

        // Setup collect button
        if (collectButton != null)
        {
            collectButton.onClick.RemoveAllListeners();
            collectButton.onClick.AddListener(() => {
                Debug.Log("Collect button clicked");
                if (currentBuilding != null)
                {
                    currentBuilding.CollectRewards();
                    UpdateUI();
                }
            });
        }

        // Setup NFT slots
        for (int i = 0; i < nftSlots.Length; i++)
        {
            int slotIndex = i; // Capture for lambda
            if (nftSlots[i] != null)
            {
                var slot = nftSlots[i];
                
                // Setup stake button
                if (slot.StakeButton != null)
                {
                    slot.StakeButton.onClick.RemoveAllListeners();
                    slot.StakeButton.onClick.AddListener(() => OnStakeButtonClicked(slotIndex));
                }

                // Setup unstake button
                if (slot.UnstakeButton != null)
                {
                    slot.UnstakeButton.onClick.RemoveAllListeners();
                    slot.UnstakeButton.onClick.AddListener(() => OnUnstakeButtonClicked(slotIndex));
                }
            }
        }

        // Initialize UI state
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (collectButton != null) collectButton.gameObject.SetActive(false);
        Hide(); // Start hidden
    }

    private void OnStakeButtonClicked(int slotIndex)
    {
        if (currentBuilding == null || !currentBuilding.IsActive)
        {
            Debug.LogError("Cannot stake - building not active or not selected");
            return;
        }

        // For testing, create a dummy Character NFT
        var testNFT = new CharacterNFT
        {
            Id = $"test_char_{slotIndex}",
            Name = $"Test Character {slotIndex}",
            Icon = null, // You'll need to assign a proper icon
            SpeedModifier = 0.2f,
            AmountModifier = 0.3f
        };

        Debug.Log($"Attempting to stake Character NFT in slot {slotIndex}");
        currentBuilding.StakeCharacter(testNFT);
        UpdateUI();
    }

    private void OnUnstakeButtonClicked(int slotIndex)
    {
        if (currentBuilding == null)
        {
            Debug.LogError("Cannot unstake - no building selected");
            return;
        }

        var stakedCharacters = currentBuilding.StakedCharacters;
        if (slotIndex < stakedCharacters.Count)
        {
            Debug.Log($"Attempting to unstake Character NFT from slot {slotIndex}");
            currentBuilding.UnstakeCharacter(stakedCharacters[slotIndex]);
            UpdateUI();
        }
    }

    // Update NFT slots in UpdateUI method
    private void UpdateNFTSlots()
    {
        var stakedCharacters = currentBuilding.StakedCharacters;
        
        for (int i = 0; i < nftSlots.Length; i++)
        {
            if (nftSlots[i] != null)
            {
                bool hasNFT = i < stakedCharacters.Count;
                if (hasNFT)
                {
                    nftSlots[i].StakeCharacter(stakedCharacters[i]);
                }
                else
                {
                    nftSlots[i].UnstakeCharacter();
                }
            }
        }
    }

    public void Show(Building building)
    {
        Debug.Log($"Showing popup for building: {building.name}");
        currentBuilding = building;
        
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            UpdateUI();
        }
        else
        {
            Debug.LogError("Cannot show popup - popupPanel is null!");
        }
    }

    public void Hide()
    {
        Debug.Log("Hiding building popup");
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            currentBuilding = null;
        }
    }

    public void UpdateTimer(float time)
    {
        if (!popupPanel || !popupPanel.activeSelf) return;
        
        if (timerText != null)
        {
            if (time > 0)
            {
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
                timerText.gameObject.SetActive(true);
                
                if (collectButton != null)
                {
                    collectButton.gameObject.SetActive(false);
                }
                Debug.Log($"Updated timer: {minutes:00}:{seconds:00}");
            }
            else
            {
                ShowCollectButton();
            }
        }
    }

    public void ShowCollectButton()
    {
        if (!popupPanel || !popupPanel.activeSelf) return;
        
        Debug.Log("Showing collect button");
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
        
        if (collectButton != null)
        {
            collectButton.gameObject.SetActive(true);
        }
    }

    public void UpdateUI()
    {
        if (currentBuilding == null)
        {
            Debug.LogWarning("Trying to update UI with no building selected");
            return;
        }

        // Update title
        if (titleText != null)
        {
            titleText.text = "Vault";
        }

        // Update info text
        if (infoText != null)
        {
            string status = !currentBuilding.IsActive ? "Inactive - Needs Zone NFT" :
                           currentBuilding.StakedCharacters.Count == 0 ? "Needs Character NFT to start production" :
                           currentBuilding.CanCollect ? "Production complete!" :
                           "Producing...";

            infoText.text = $"Building Status: {status}\n\n" +
                          $"Staked Characters: {currentBuilding.StakedCharacters.Count}/3\n" +
                          $"Base Fortune: {currentBuilding.BaseFortuneAmount}\n\n" +
                          "Stake Character NFTs to boost:\n" +
                          "- Production Speed\n" +
                          "- Fortune Amount";
        }

        // Update NFT slots
        UpdateNFTSlots();

        // Update timer/collect button state
        if (currentBuilding.CanCollect)
        {
            ShowCollectButton();
        }
        else if (currentBuilding.IsProducing)
        {
            UpdateTimer(currentBuilding.CurrentProductionTime);
        }
        else
        {
            // Hide both timer and collect button when not producing
            if (timerText != null) timerText.gameObject.SetActive(false);
            if (collectButton != null) collectButton.gameObject.SetActive(false);
        }
    }
} 