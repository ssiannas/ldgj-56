using Menus;
using Menus.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    /// <summary>
    /// Use the Show and Hide methods to display or remove Menu UIs
    ///
    /// Use OnCharacterSelected event to add listeners for when the player selects a new character from
    /// the Start Menu
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [SerializeField] private GameObject startMenu;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject tutorialMenu;
        [SerializeField] private GameObject scoreBoard;
        [SerializeField] private GameObject gameOverMenu;

        [SerializeField] private AudioChannel _audioChannel;

        public UnityEvent<CharacterChoiceSO> OnCharacterSelected =>
            startMenu.GetComponent<StartMenu>().OnCharacterSelected;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            scoreBoard = GameObject.FindGameObjectWithTag("Scoreboard");
        }

        public void HideMenu()
        {
            startMenu.SetActive(false);
            pauseMenu.SetActive(false);
            tutorialMenu.SetActive(false);
            gameOverMenu.SetActive(false);
        }

        public void ShowStartMenu()
        {
            HideMenu();
            startMenu.SetActive(true);
            if (!_audioChannel.IsAudioPlaying("MainMenuTheme"))
            {
                _audioChannel.PlayAudio("MainMenuTheme");
            }
        }

        public void ShowPauseMenu()
        {
            HideMenu();
            pauseMenu.SetActive(true);
        }

        public void ShowTutorialMenu()
        {
            HideMenu();
            tutorialMenu.SetActive(true);
        }

        public void ShowGameOverMenu(int score, int highScore, bool isNewHighScore)
        {
            HideMenu();
            gameOverMenu.SetActive(true);
            gameOverMenu.GetComponent<GameOverMenu>().SetScore(score, highScore, isNewHighScore);
        }

        public void UpdateScore(float score)
        {
            scoreBoard?.GetComponent<ScoreController>().UpdateScore(score);
        }

        public void ShowScoreText(bool show)
        {
            scoreBoard.SetActive(show);
        }
    }
}