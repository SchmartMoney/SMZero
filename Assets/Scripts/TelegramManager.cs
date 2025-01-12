using UnityEngine;
using System.Runtime.InteropServices;

public class TelegramManager : MonoBehaviour
{
    private static TelegramManager instance;
    public static TelegramManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TelegramManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("TelegramManager");
                    instance = go.AddComponent<TelegramManager>();
                }
            }
            return instance;
        }
    }

    public bool IsTelegramApp { get; private set; }
    public string UserId { get; private set; }

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool InitTelegramWebApp();

    [DllImport("__Internal")]
    private static extern string GetTelegramUserId();

    [DllImport("__Internal")]
    private static extern bool IsTelegramWebApp();
    #endif

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTelegram();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTelegram()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            IsTelegramApp = IsTelegramWebApp();
            if (IsTelegramApp)
            {
                if (InitTelegramWebApp())
                {
                    UserId = GetTelegramUserId();
                    Debug.Log($"Telegram initialized. User ID: {UserId}");
                }
                else
                {
                    Debug.LogError("Failed to initialize Telegram WebApp");
                }
            }
            else
            {
                Debug.Log("Not running as Telegram WebApp");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing Telegram: {e.Message}");
        }
        #else
        IsTelegramApp = false;
        UserId = "0";
        Debug.Log("Running in non-WebGL platform or Editor");
        #endif
    }

    public bool IsInitialized()
    {
        return IsTelegramApp && !string.IsNullOrEmpty(UserId) && UserId != "0";
    }
} 