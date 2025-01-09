using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseProductionTime = 10f; // 10 seconds for testing
    [SerializeField] private int baseFortuneAmount = 100;

    [Header("Colors")]
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color inactiveColor = Color.gray;
    
    private bool isActive;
    private List<CharacterNFT> stakedCharacters = new List<CharacterNFT>();
    private float currentProductionTime;
    private bool isProducing;
    private bool canCollect;
    private LocationHighlight locationHighlight;

    // Public properties for UI
    public bool IsActive => isActive;
    public bool IsProducing => isProducing;
    public bool CanCollect => canCollect;
    public float CurrentProductionTime => currentProductionTime;
    public IReadOnlyList<CharacterNFT> StakedCharacters => stakedCharacters.AsReadOnly();
    public int BaseFortuneAmount => baseFortuneAmount;

    private void Awake()
    {
        locationHighlight = GetComponent<LocationHighlight>();
        if (locationHighlight == null)
        {
            Debug.LogError($"Building {gameObject.name} needs a LocationHighlight component!");
        }
    }

    private void Start()
    {
        // Ensure we have required components
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError($"Building {gameObject.name} needs a Collider component!");
        }

        // Start inactive by default - requires Zone NFT to activate
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        isActive = active;
        Debug.Log($"Building {gameObject.name} active state set to: {active}");

        // Update LocationHighlight color
        if (locationHighlight != null)
        {
            locationHighlight.SetHighlightColor(active ? activeColor : inactiveColor);
            locationHighlight.SetInteractable(active);
        }

        // If deactivated, stop production
        if (!active)
        {
            StopProduction();
        }
        // If activated and has characters, start production
        else if (stakedCharacters.Count > 0)
        {
            StartProduction();
        }
    }

    private void StopProduction()
    {
        isProducing = false;
        canCollect = false;
        currentProductionTime = 0;
        if (BuildingPopup.Instance != null)
        {
            BuildingPopup.Instance.UpdateUI();
        }
    }

    private void OnMouseDown()
    {
        HandleInteraction();
    }

    private void Update()
    {
        // Handle touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                HandleInteraction();
            }
        }

        // Production timer update
        if (isProducing && !canCollect)
        {
            currentProductionTime -= Time.deltaTime;
            Debug.Log($"Timer: {currentProductionTime:F1}"); // Debug log for timer

            if (BuildingPopup.Instance != null)
            {
                BuildingPopup.Instance.UpdateTimer(currentProductionTime);
            }

            if (currentProductionTime <= 0)
            {
                currentProductionTime = 0;
                canCollect = true;
                isProducing = false;
                if (BuildingPopup.Instance != null)
                {
                    BuildingPopup.Instance.ShowCollectButton();
                }
                Debug.Log("Production complete - ready to collect");
            }
        }
    }

    private void HandleInteraction()
    {
        if (!isActive)
        {
            Debug.Log($"Building {gameObject.name} interaction ignored - building not active");
            return;
        }

        Debug.Log($"Building {gameObject.name} clicked/touched");
        if (BuildingPopup.Instance != null)
        {
            BuildingPopup.Instance.Show(this);
        }
        else
        {
            Debug.LogError("BuildingPopup.Instance is null!");
        }
    }

    public void StakeCharacter(CharacterNFT character)
    {
        if (!isActive)
        {
            Debug.Log("Cannot stake character - building not active (needs Zone NFT)");
            return;
        }

        if (stakedCharacters.Count >= 3)
        {
            Debug.Log("Cannot stake character - maximum number of characters reached");
            return;
        }
        
        stakedCharacters.Add(character);
        UpdateProductionModifiers();
        
        // Start production when first character is staked
        if (!isProducing && stakedCharacters.Count == 1)
        {
            StartProduction();
        }
    }

    public void UnstakeCharacter(CharacterNFT character)
    {
        if (stakedCharacters.Remove(character))
        {
            Debug.Log($"Character unstaked from building {gameObject.name}");
            UpdateProductionModifiers();

            // Stop production if no characters left
            if (stakedCharacters.Count == 0)
            {
                StopProduction();
            }
        }
    }

    private void StartProduction()
    {
        if (!isActive || stakedCharacters.Count == 0)
        {
            Debug.Log("Cannot start production - building inactive or no characters staked");
            return;
        }

        isProducing = true;
        currentProductionTime = baseProductionTime;
        canCollect = false;
        Debug.Log($"Production started in building {gameObject.name}. Time: {baseProductionTime}s");
    }

    private void UpdateProductionModifiers()
    {
        float speedModifier = 1f;
        float amountModifier = 1f;

        foreach (var character in stakedCharacters)
        {
            speedModifier += character.SpeedModifier;
            amountModifier += character.AmountModifier;
        }

        currentProductionTime = baseProductionTime / speedModifier;
        Debug.Log($"Production modifiers updated. Speed: {speedModifier}x, Amount: {amountModifier}x");
    }

    public void CollectRewards()
    {
        if (!canCollect)
        {
            Debug.Log("Cannot collect - production not complete");
            return;
        }

        float amountModifier = 1f;
        foreach (var character in stakedCharacters)
        {
            amountModifier += character.AmountModifier;
        }

        int fortuneAmount = Mathf.RoundToInt(baseFortuneAmount * amountModifier);
        Debug.Log($"Collected {fortuneAmount} fortune from building {gameObject.name}");

        StartProduction(); // Restart production
    }
} 