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
    
    private Building activeBuilding;
    private ZoneNFT stakedZoneNFT;

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
        }
    }

    public void ReturnToMap()
    {
        SceneManager.LoadScene("MainMap");
    }

    public void StakeZoneNFT(ZoneNFT nft)
    {
        stakedZoneNFT = nft;
        zoneUI.UpdateZoneSlot(0, nft);
        activeBuilding?.SetActive(true);
    }

    public void UnstakeZoneNFT()
    {
        stakedZoneNFT = null;
        zoneUI.UpdateZoneSlot(0, null);
        activeBuilding?.SetActive(false);
    }
} 