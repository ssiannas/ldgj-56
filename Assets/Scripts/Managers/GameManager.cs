using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void StartGame()
        {
            Debug.Log("Start Game");
        }

        public void ExitGame()
        {
            Debug.Log("Exitting game");
            Application.Quit(0);
        }
    }
}