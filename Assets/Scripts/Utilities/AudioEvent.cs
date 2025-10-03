using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper component for triggering common audio actions from UnityEvents.
/// </summary>
public class AudioEvent : MonoBehaviour
{
	[Tooltip("Fade duration used when transitioning music.")]
	public float FadeTime = 1;

	/// <summary>
	/// Sets the fade time for subsequent audio actions.
	/// </summary>
	public void SetFadeTime(float fadeTime)
	{
		FadeTime = fadeTime;
	}

	/// <summary>
	/// Plays music with crossfade.
	/// </summary>
	public void PlayMusic(AudioClip clip)
	{
		AudioManager.PlayMusic(clip, FadeTime);
	}

	/// <summary>
	/// Plays music with crossfade, syncing playhead with current track.
	/// </summary>
	public void PlayMusicSync(AudioClip clip)
	{
		AudioManager.PlayMusic(clip, FadeTime, true);
	}

	/// <summary>
	/// Plays a 3D sound effect at this transform's position.
	/// </summary>
	public void PlaySound(AudioClip clip)
	{
		AudioManager.PlaySound(this, clip, SoundType.SoundEffect, this.transform.position);
	}

	/// <summary>
	/// Plays a short stinger (routed to Dialogue/Interface by default).
	/// </summary>
	public void PlayStinger(AudioClip clip)
	{
		AudioManager.PlaySound(this, clip, SoundType.Dialogue);
	}
}
