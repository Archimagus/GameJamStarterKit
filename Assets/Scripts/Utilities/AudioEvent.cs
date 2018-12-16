using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEvent : MonoBehaviour
{
	public float FadeTime = 1;

	public void SetFadeTime (float fadeTime)
	{
		FadeTime = fadeTime;
	}

	public void PlayMusic (AudioClip clip)
	{
		AudioManager.PlayMusic(clip, FadeTime);
	}

	public void PlayMusicSync(AudioClip clip)
	{
		AudioManager.PlayMusic(clip, FadeTime, true);
	}

	public void PlaySound (AudioClip clip)
	{
		AudioManager.PlaySound(this, clip, SoundType.SoundEffect, this.transform.position);
	}

	public void PlayStinger (AudioClip clip)
	{
		AudioManager.PlaySound(this, clip, SoundType.Dialogue);
	}
}
