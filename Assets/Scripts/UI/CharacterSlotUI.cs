using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    [SerializeField] private Image characterIcon;
    [SerializeField] private Button stakeButton;
    [SerializeField] private Button unstakeButton;

    private CharacterNFT stakedCharacter;

    public void StakeCharacter(CharacterNFT character)
    {
        stakedCharacter = character;
        characterIcon.sprite = character.Icon;
        stakeButton.gameObject.SetActive(false);
        unstakeButton.gameObject.SetActive(true);
    }

    public void UnstakeCharacter()
    {
        stakedCharacter = null;
        characterIcon.sprite = null;
        stakeButton.gameObject.SetActive(true);
        unstakeButton.gameObject.SetActive(false);
    }
} 