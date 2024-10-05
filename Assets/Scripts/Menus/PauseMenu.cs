using System;
using UnityEngine;

namespace Menus
{
    public class PauseMenu : MonoBehaviour
    {
        public void ResumeButtonHandler()
        {
            GameControler.Instance.ResumeGame();
        }
        
        public void MusicToggleButtonHandler()
        {
            throw new NotImplementedException("Toggling music on/off not yet implemented");
        }
        
        public void QuitButtonHandler()
        {
            GameControler.Instance.QuitToStartMenu();
        }
    }
}
