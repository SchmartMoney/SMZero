using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingPopup : MonoBehaviour
{
    public static BuildingPopup Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button collectButton;
    [SerializeField] private CharacterSlotUI[] characterSlots;

    private Building currentBuilding;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(Building building)
    {
        currentBuilding = building;
        popupPanel.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
        currentBuilding = null;
    }

    public void UpdateTimer(float time)
    {
        if (!popupPanel.activeSelf) return;
        
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void ShowCollectButton()
    {
        timerText.gameObject.SetActive(false);
        collectButton.gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        // Update description, 3D model, etc.
    }
} 