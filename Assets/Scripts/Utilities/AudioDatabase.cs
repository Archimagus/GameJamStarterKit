using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioDatabase : ScriptableObject, ISerializationCallbackReceiver
{
	[Header("Auto Generated, Don't Change")]
	[ReadOnly] [SerializeField] private AudioClip[] _audioClips = null;
	[ReadOnly] [SerializeField] private SoundType[] _clipTypes = null;

	public Dictionary<AudioClips, AudioClipData> AudioClips { get; } = new Dictionary<AudioClips, AudioClipData>();
	public Dictionary<AudioClip, SoundType> ClipTypes { get; } = new Dictionary<AudioClip, SoundType>();

	public void OnAfterDeserialize()
	{
		AudioClips.Clear();
		ClipTypes.Clear();
		for (int i = 0; i < _audioClips.Length; i++)
		{
			AudioClips.Add((AudioClips)i, new AudioClipData(_audioClips[i], _clipTypes[i]));
			ClipTypes.Add(_audioClips[i], _clipTypes[i]);
		}
	}

	public void OnBeforeSerialize()
	{
		try
		{
			var clips = new List<AudioClip>();
			var types = new List<SoundType>();
			var clipsArray = System.Enum.GetValues(typeof(AudioClips));
			if (clipsArray.Length > 0)
			{
				foreach (var c in (AudioClips[])clipsArray)
				{
					clips.Add(AudioClips[c].Clip);
					types.Add(AudioClips[c].SoundType);
				}
				_audioClips = clips.ToArray();
				_clipTypes = types.ToArray();
			}
		}
		catch { }
	}
}

public class AudioClipData
{
	public AudioClip Clip;
	public SoundType SoundType;
	public AudioClipData(AudioClip clip, SoundType soundType)
	{
		Clip = clip;
		SoundType = soundType;
	}
}