using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LocationPopupUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button closeButton;

    private UnityAction onEnterAction;
    private UnityAction onLeaveAction;

    private void Awake()
    {
        ValidateComponents();
        SetupUI();
        Hide(); // Start hidden
    }

    private void ValidateComponents()
    {
        if (popupPanel == null) Debug.LogError("PopupPanel not assigned!");
        if (titleText == null) Debug.LogError("TitleText not assigned!");
        if (descriptionText == null) Debug.LogError("DescriptionText not assigned!");
        if (enterButton == null) Debug.LogError("EnterButton not assigned!");
        if (leaveButton == null) Debug.LogError("LeaveButton not assigned!");
        if (closeButton == null) Debug.LogError("CloseButton not assigned!");
    }

    private void SetupUI()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Hide);
        }

        if (leaveButton != null)
        {
            leaveButton.onClick.RemoveAllListeners();
            leaveButton.onClick.AddListener(() => {
                onLeaveAction?.Invoke();
            });
        }

        if (enterButton != null)
        {
            enterButton.onClick.RemoveAllListeners();
            enterButton.onClick.AddListener(() => {
                onEnterAction?.Invoke();
            });
        }
    }

    public void Show(LocationData locationData, LocationUIData uiData, UnityAction onEnter, UnityAction onLeave)
    {
        if (locationData == null || uiData == null)
        {
            Debug.LogError("Cannot show popup - data is null!");
            return;
        }

        // Store callbacks
        onEnterAction = onEnter;
        onLeaveAction = onLeave;

        // Update UI
        if (titleText != null)
        {
            titleText.text = uiData.title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = uiData.description;
        }

        // Show/hide buttons based on UI data
        if (enterButton != null)
        {
            enterButton.gameObject.SetActive(uiData.showEnterButton);
        }

        if (leaveButton != null)
        {
            leaveButton.gameObject.SetActive(uiData.showLeaveButton);
        }

        // Show the popup
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
    }

    public void Hide()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        // Clear callbacks
        onEnterAction = null;
        onLeaveAction = null;
    }
} 