using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public bool PlayMusic { get; private set; } = true;
		public List<Sound> sounds;
		[SerializeField] AudioChannel channel;

		private  void Awake()
		{
			channel.OnAudioRequested += Play;
			channel.OnAudioStopped += Stop;
			channel.OnIsAudioPlaying = IsPlaying;

			foreach (var s in sounds)
			{
				var source = gameObject.AddComponent<AudioSource>();
				source.clip = s.clip;
				source.volume = s.volume;
				source.pitch = s.pitch;
				source.loop = s.loop;
				s.source = source;
				
			}
        }

		public void Play(string soundName)
		{
			var s = sounds.Find(sound => sound.soundName == soundName);
			s.source.Play();
		}

		public void Stop(string soundName)
		{
			var s = sounds.Find(sound => sound.soundName == soundName);
			s.source.Stop();
		}

		public bool IsPlaying(string soundName)
		{
			var s = sounds.Find(sound => sound.soundName == soundName);
			return s.source.isPlaying;
		}
    }
}