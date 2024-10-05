using Menus;
using Menus.Characters;
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
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { get; private set; }
        [SerializeField] private GameObject startMenu;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject tutorialMenu;

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

        public void HideMenu()
        {
            startMenu.SetActive(false);
            pauseMenu.SetActive(false);
            tutorialMenu.SetActive(false);
        }

        public void ShowStartMenu()
        {
            HideMenu();
            startMenu.SetActive(true);
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
    }
}