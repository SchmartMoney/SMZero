using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TelegramUserDisplay : MonoBehaviour
{
    private TextMeshProUGUI userIdText;
    private TelegramManager telegramManager;

    private void Start()
    {
        userIdText = GetComponent<TextMeshProUGUI>();
        telegramManager = TelegramManager.Instance;

        UpdateUserIdDisplay();
    }

    private void UpdateUserIdDisplay()
    {
        if (telegramManager.IsInitialized())
        {
            userIdText.text = $"Telegram ID: {telegramManager.UserId}";
            userIdText.color = Color.green;
        }
        else if (telegramManager.IsTelegramApp)
        {
            userIdText.text = "Initializing Telegram...";
            userIdText.color = Color.yellow;
        }
        else
        {
            userIdText.text = "Not running in Telegram";
            userIdText.color = Color.gray;
        }
    }
} 