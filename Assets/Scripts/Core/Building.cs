using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseProductionTime = 300f; // 5 minutes
    [SerializeField] private int baseFortuneAmount = 100;
    
    private bool isActive;
    private List<CharacterNFT> stakedCharacters = new List<CharacterNFT>();
    private float currentProductionTime;
    private bool isProducing;
    private bool canCollect;

    public void SetActive(bool active)
    {
        isActive = active;
        // Later: Update visual state
    }

    public void OnClick()
    {
        if (!isActive) return;
        BuildingPopup.Instance.Show(this);
    }

    public void StakeCharacter(CharacterNFT character)
    {
        if (stakedCharacters.Count >= 3) return;
        
        stakedCharacters.Add(character);
        UpdateProductionModifiers();
        
        if (!isProducing && stakedCharacters.Count > 0)
        {
            StartProduction();
        }
    }

    public void UnstakeCharacter(CharacterNFT character)
    {
        stakedCharacters.Remove(character);
        UpdateProductionModifiers();
    }

    private void StartProduction()
    {
        isProducing = true;
        currentProductionTime = baseProductionTime;
        canCollect = false;
    }

    private void UpdateProductionModifiers()
    {
        // Calculate modifiers based on staked characters
        float speedModifier = 1f;
        float amountModifier = 1f;

        foreach (var character in stakedCharacters)
        {
            speedModifier += character.SpeedModifier;
            amountModifier += character.AmountModifier;
        }

        currentProductionTime = baseProductionTime / speedModifier;
        // Update UI
    }

    private void Update()
    {
        if (!isProducing || canCollect) return;

        currentProductionTime -= Time.deltaTime;
        BuildingPopup.Instance.UpdateTimer(currentProductionTime);

        if (currentProductionTime <= 0)
        {
            canCollect = true;
            BuildingPopup.Instance.ShowCollectButton();
        }
    }

    public void CollectRewards()
    {
        if (!canCollect) return;

        // Calculate final fortune amount with modifiers
        float amountModifier = 1f;
        foreach (var character in stakedCharacters)
        {
            amountModifier += character.AmountModifier;
        }

        int fortuneAmount = Mathf.RoundToInt(baseFortuneAmount * amountModifier);
        // Add fortune to player's account

        StartProduction(); // Restart production
    }
} 