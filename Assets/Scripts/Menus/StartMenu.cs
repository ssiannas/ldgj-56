using System.Collections.Generic;
using Menus.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menus
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private List<CharacterChoiceSO> characterChoices;
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private Image characterImage;
        private int characterIx = 0;
        public CharacterChoiceSO CurrentCharacter => characterChoices[characterIx];

        [SerializeField] public UnityEvent<CharacterChoiceSO> OnCharacterSelected = new();

        [field: SerializeField] public PlayerPersistentState PersistentState { get; private set; }

        [field: SerializeField] public AudioChannel AudioChan { get; private set; }
        [SerializeField] private TMP_Text musicToggleText;

        private void Start()
        {
            InitElements();
        }

        private void InitElements()
        {
            // Character Select init
            characterIx = characterChoices.FindIndex(x => x == PersistentState.CharacterChoice);
            DisplayCharacter(CurrentCharacter);
            OnCharacterSelected.AddListener(character => PersistentState.CharacterChoice = character);
            OnCharacterSelected.Invoke(CurrentCharacter);

            // Music init
            UpdateBgMusicState();
        }

        private void UpdateBgMusicState()
        {
            if (PersistentState.MusicMuted)
            {
                AudioChan.StopAudio("MainMenuTheme");
            }
            else
            {
                if (!AudioChan.IsAudioPlaying("MainMenuTheme"))
                {
                    AudioChan.PlayAudio("MainMenuTheme");
                }
            }

            var stateStr = PersistentState.MusicMuted ? "OFF" : "ON";
            musicToggleText.text = $"Music: {stateStr}";
        }

        private void DisplayCharacter(CharacterChoiceSO character)
        {
            characterNameText.text = character.name;
            characterImage.GetComponent<Animator>().runtimeAnimatorController = character.mainMenuAnimation;
        }

        public void ChooseNextCharacter()
        {
            characterIx = (characterIx + 1) % characterChoices.Count;
            DisplayCharacter(CurrentCharacter);
            OnCharacterSelected.Invoke(CurrentCharacter);
        }

        public void ChoosePreviousCharacter()
        {
            characterIx = (characterChoices.Count + characterIx - 1) % characterChoices.Count;
            DisplayCharacter(CurrentCharacter);
            OnCharacterSelected.Invoke(CurrentCharacter);
        }

        public void StartButtonHandler()
        {
            GameController.Instance.StartGame();
        }

        public void TutorialButtonHandler()
        {
            Managers.UIManager.Instance.ShowTutorialMenu();
        }

        public void MusicToggleButtonHandler()
        {
            PersistentState.MusicMuted = !PersistentState.MusicMuted;
            UpdateBgMusicState();
        }

        public void ExitButtonHandler()
        {
            GameController.Instance.ExitGame();
        }
    }
}