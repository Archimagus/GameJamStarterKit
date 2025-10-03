using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEventListner : GameEventListner
{
	public float FadeTime = 1;

	private void OnValidate()
	{
		if (Response.GetPersistentEventCount() == 0)
		{
#if UNITY_EDITOR
			var clip = AudioClip.Create("", 1, 1, 44000, false);
			UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<AudioClip>(Response, PlaySound, clip);
			DestroyImmediate(clip);
#endif
		}
	}
	public void SetFadeTime(float fadeTime)
	{
		FadeTime = fadeTime;
	}

	public void PlayMusic(AudioClip clip)
	{
		AudioManager.PlayMusic(clip, FadeTime);
	}

	public void PlayMusicSync(AudioClip clip)
	{
		AudioManager.PlayMusic(clip, FadeTime, true);
	}

	public void PlaySound(AudioClip clip)
	{
		AudioManager.PlaySound(this, clip, SoundType.SoundEffect, this.transform.position);
	}

	public void PlayStinger(AudioClip clip)
	{
		AudioManager.PlaySound(this, clip, SoundType.Dialogue);
	}
}
