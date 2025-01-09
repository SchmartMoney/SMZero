using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    [Header("Zone Settings")]
    [SerializeField] private int totalSlots = 10;
    [SerializeField] private int activeSlots = 1;
    
    [Header("References")]
    [SerializeField] private ZoneUI zoneUI;
    [SerializeField] private BuildingPopup buildingPopup;
    [SerializeField] private Building activeBuilding;
    
    private ZoneNFT stakedZoneNFT;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ValidateReferences();
            InitializeZone();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ValidateReferences()
    {
        if (zoneUI == null)
        {
            Debug.LogError("ZoneUI reference is missing in ZoneManager!");
            zoneUI = FindObjectOfType<ZoneUI>();
        }

        if (buildingPopup == null)
        {
            Debug.LogError("BuildingPopup reference is missing in ZoneManager!");
            buildingPopup = FindObjectOfType<BuildingPopup>();
        }

        if (activeBuilding == null)
        {
            Debug.LogError("Active Building reference is missing in ZoneManager!");
            activeBuilding = FindObjectOfType<Building>();
        }
    }

    private void InitializeZone()
    {
        if (isInitialized) return;

        Debug.Log("Initializing Zone Manager...");
        
        // Initialize the zone
        if (zoneUI != null)
        {
            zoneUI.SetZoneName("Vault Avenue");
        }

        // Ensure building starts with correct state
        if (activeBuilding != null)
        {
            bool shouldBeActive = stakedZoneNFT != null;
            Debug.Log($"Initial building state: {(shouldBeActive ? "Active" : "Inactive")}");
            activeBuilding.SetActive(shouldBeActive);
        }

        isInitialized = true;
    }

    public void ReturnToMap()
    {
        Debug.Log("Returning to main map...");
        SceneManager.LoadScene("MainScene");
    }

    public void StakeZoneNFT(ZoneNFT nft)
    {
        if (nft == null)
        {
            Debug.LogError("Attempting to stake null ZoneNFT!");
            return;
        }

        Debug.Log($"Staking Zone NFT: {nft.Name}");
        stakedZoneNFT = nft;
        
        if (zoneUI != null)
        {
            zoneUI.UpdateZoneSlot(0, nft);
        }
        else
        {
            Debug.LogError("ZoneUI is null in ZoneManager!");
        }

        if (activeBuilding != null)
        {
            Debug.Log("Activating building...");
            activeBuilding.SetActive(true);
        }
        else
        {
            Debug.LogError("Active Building is null in ZoneManager!");
        }
    }

    public void UnstakeZoneNFT()
    {
        Debug.Log("Unstaking Zone NFT");
        stakedZoneNFT = null;
        
        if (zoneUI != null)
        {
            zoneUI.UpdateZoneSlot(0, null);
        }

        if (activeBuilding != null)
        {
            activeBuilding.SetActive(false);
        }
    }
} 