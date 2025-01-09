using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private Button stakeButton;
    [SerializeField] private Button unstakeButton;

    private CharacterNFT stakedCharacter;

    private void Awake()
    {
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (characterIcon == null)
        {
            Debug.LogError($"Character Icon is missing on {gameObject.name}!");
            characterIcon = transform.Find("NFTIcon")?.GetComponent<Image>();
        }

        if (stakeButton == null)
        {
            Debug.LogError($"Stake Button is missing on {gameObject.name}!");
            stakeButton = transform.Find("StakeButton")?.GetComponent<Button>();
        }

        if (unstakeButton == null)
        {
            Debug.LogError($"Unstake Button is missing on {gameObject.name}!");
            unstakeButton = transform.Find("UnstakeButton")?.GetComponent<Button>();
        }
    }

    // Public properties
    public Button StakeButton => stakeButton;
    public Button UnstakeButton => unstakeButton;
    public CharacterNFT StakedCharacter => stakedCharacter;

    public void StakeCharacter(CharacterNFT character)
    {
        if (character == null)
        {
            Debug.LogError("Attempting to stake null character!");
            return;
        }

        stakedCharacter = character;
        
        if (characterIcon != null)
        {
            characterIcon.sprite = character.Icon;
            characterIcon.gameObject.SetActive(true);
        }

        if (stakeButton != null)
        {
            stakeButton.gameObject.SetActive(false);
        }

        if (unstakeButton != null)
        {
            unstakeButton.gameObject.SetActive(true);
        }
    }

    public void UnstakeCharacter()
    {
        stakedCharacter = null;
        
        if (characterIcon != null)
        {
            characterIcon.sprite = null;
            characterIcon.gameObject.SetActive(false);
        }

        if (stakeButton != null)
        {
            stakeButton.gameObject.SetActive(true);
        }

        if (unstakeButton != null)
        {
            unstakeButton.gameObject.SetActive(false);
        }
    }
} 