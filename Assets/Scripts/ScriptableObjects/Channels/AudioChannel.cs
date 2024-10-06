using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AudioChannels", menuName = "ScriptableObjects/Channels/AudioChannel", order = 1)]

public class AudioChannel : ScriptableObject
{

    public UnityAction<string> OnAudioRequested;
    public UnityAction<string> OnAudioStopped;
	public Func<string, bool> OnIsAudioPlaying;

    public void PlayAudio(string soundName)
    {
        if (OnAudioRequested != null) {
            OnAudioRequested.Invoke(soundName);
        }
        else
        {
            Debug.Log("No AudioManager registered");
        }
    }

	public void StopAudio(string soundName)
	{
		if (OnAudioStopped != null)
		{
			OnAudioStopped.Invoke(soundName);
		}
		else
		{
			Debug.Log("No AudioManager registered");
		}
	}

	public bool IsAudioPlaying(string soundName)
	{
		return	OnIsAudioPlaying(soundName);
	}

}
