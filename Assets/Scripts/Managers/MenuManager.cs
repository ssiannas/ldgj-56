using UnityEngine;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { get; private set; }
        [SerializeField] private GameObject startMenu;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject tutorialMenu;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
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