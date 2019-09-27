using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AudioDatabase : ScriptableObject, ISerializationCallbackReceiver
{
	[Header("Auto Generated, Don't Change")]
	[ReadOnly] [SerializeField] private AudioClip[] _audioClips = null;
	[ReadOnly] [SerializeField] private int[] _clipIds = null;
	[ReadOnly] [SerializeField] private SoundType[] _clipTypes = null;

	public Dictionary<AudioClips, AudioClipData> AudioClips { get; } = new Dictionary<AudioClips, AudioClipData>();
	public Dictionary<AudioClip, SoundType> ClipTypes { get; } = new Dictionary<AudioClip, SoundType>();

	public void OnAfterDeserialize()
	{
		AudioClips.Clear();
		ClipTypes.Clear();
		for (int i = 0; i < _audioClips.Length; i++)
		{
			AudioClips.Add((AudioClips)_clipIds[i], new AudioClipData(_audioClips[i], _clipTypes[i]));
			ClipTypes.Add(_audioClips[i], _clipTypes[i]);
		}
	}

	public void OnBeforeSerialize()
	{
		try
		{
			var clips = new List<AudioClip>();
			var types = new List<SoundType>();
			var ids = new List<int>();
			if (AudioClips.Any())
			{
				foreach (var c in AudioClips)
				{
					clips.Add(c.Value.Clip);
					types.Add(c.Value.SoundType);
					ids.Add((int)c.Key);
				}
				_audioClips = clips.ToArray();
				_clipTypes = types.ToArray();
				_clipIds = ids.ToArray();
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