using System;
using UnityEngine;

namespace Menus
{
    public class PauseMenu : MonoBehaviour
    {
        public void ResumeButtonHandler()
        {
            GameController.Instance.ResumeGame();
        }
        
        public void MusicToggleButtonHandler()
        {
            throw new NotImplementedException("Toggling music on/off not yet implemented");
        }
        
        public void QuitButtonHandler()
        {
            GameController.Instance.QuitToStartMenu();
        }
    }
}
