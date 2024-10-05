using System;
using Menus.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menus
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private CharacterChoiceSO[] characterChoices;
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private Image characterImage;
        private int characterIx = 0;
        public CharacterChoiceSO CurrentCharacter => characterChoices[characterIx];

        [SerializeField] public UnityEvent<CharacterChoiceSO> OnCharacterSelected = new();


        private void Awake()
        {
            DisplayCharacter(CurrentCharacter);
            OnCharacterSelected.Invoke(CurrentCharacter);
        }

        private void DisplayCharacter(CharacterChoiceSO character)
        {
            characterNameText.text = character.name;
            characterImage.sprite = character.img;
        }

        public void ChooseNextCharacter()
        {
            characterIx = (characterIx + 1) % characterChoices.Length;
            DisplayCharacter(CurrentCharacter);
            OnCharacterSelected.Invoke(CurrentCharacter);
        }

        public void ChoosePreviousCharacter()
        {
            characterIx = (characterIx - 1) % characterChoices.Length;
            DisplayCharacter(CurrentCharacter);
            OnCharacterSelected.Invoke(CurrentCharacter);
        }

        public void StartButtonHandler()
        {
            GameController.Instance.StartGame();
        }

        public void TutorialButtonHandler()
        {
            Managers.MenuManager.Instance.ShowTutorialMenu();
        }

        public void MusicToggleButtonHandler()
        {
            throw new NotImplementedException("Music toggle is not implemented yet");
        }

        public void ExitButtonHandler()
        {
            GameController.Instance.ExitGame();
        }
    }
}