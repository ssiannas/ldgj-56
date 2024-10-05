using System;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public bool PlayMusic { get; private set; } = true;

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

        public void ToggleMusic()
        {
            PlayMusic = !PlayMusic;

            Debug.Log("Toggle Music");

            throw new NotImplementedException("Should stop playing music here");
        }
    }
}