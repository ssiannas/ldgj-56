using System;
using TMPro;
using UnityEngine;

namespace Menus
{
    public class PauseMenu : MonoBehaviour
    {
        [field: SerializeField] public PlayerPersistentState PersistentState { get; private set; }

        [field: SerializeField] public AudioChannel AudioChan { get; private set; }
        [SerializeField] private TMP_Text musicToggleText;

        private void Awake()
        {
            UpdateBgMusicState();
        }

        private void OnEnable()
        {
            UpdateBgMusicState();
        }

        public void ResumeButtonHandler()
        {
            GameController.Instance.ResumeGame();
        }

        public void MusicToggleButtonHandler()
        {
            PersistentState.MusicMuted = !PersistentState.MusicMuted;
            UpdateBgMusicState();
        }

        public void QuitButtonHandler()
        {
            GameController.Instance.QuitToStartMenu();
        }

        private void UpdateBgMusicState()
        {
            if (PersistentState.MusicMuted)
            {
                AudioChan.StopAudio("Theme");
            }
            else
            {
                if (!AudioChan.IsAudioPlaying("Theme"))
                {
                    AudioChan.PlayAudio("Theme");
                }
            }

            var stateStr = PersistentState.MusicMuted ? "OFF" : "ON";
            musicToggleText.text = $"Music: {stateStr}";
        }
    }
}