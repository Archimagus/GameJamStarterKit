using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Serialized lookup between stable audio IDs and clips/sound types. Backed by arrays for serialization.
/// </summary>
public class AudioDatabase : ScriptableObject, ISerializationCallbackReceiver
{
	[Header("Auto Generated, Don't Change")]
	[ReadOnly][SerializeField] private AudioClip[] _audioClips = null;
	[ReadOnly][SerializeField] private ulong[] _clipIds = null;
	[ReadOnly][SerializeField] private SoundType[] _clipTypes = null;

	private Dictionary<ulong, AudioClipData> AudioClips { get; } = new Dictionary<ulong, AudioClipData>();
	private Dictionary<AudioClip, SoundType> ClipTypes { get; } = new Dictionary<AudioClip, SoundType>();

	public void Clear()
	{
		Debug.LogWarning("Clearing audio database");

		AudioClips.Clear();
		ClipTypes.Clear();
		_audioClips = null;
		_clipIds = null;
		_clipTypes = null;
	}
	public void Add(ulong id, AudioClip clip, SoundType type)
	{
		var ad = new AudioClipData(clip, type);
		AudioClips.Add(id, ad);
	}
	public void Remove(ulong id)
	{
		AudioClips.Remove(id);
	}

	/// <summary>
	/// Gets audio clip data by enum id.
	/// </summary>
	public AudioClipData this[AudioClips index]
	{
		get
		{
			AudioClips.TryGetValue((ulong)index, out AudioClipData data);
			return data;
		}
	}
	/// <summary>
	/// Gets the configured sound type for a specific AudioClip.
	/// </summary>
	public SoundType this[AudioClip index]
	{
		get { return ClipTypes[index]; }
	}

	public void OnAfterDeserialize()
	{
		AudioClips.Clear();
		ClipTypes.Clear();
		for (int i = 0; i < _audioClips.Length; i++)
		{
			AudioClips.Add(_clipIds[i], new AudioClipData(_audioClips[i], _clipTypes[i]));
			ClipTypes.Add(_audioClips[i], _clipTypes[i]);
		}
	}

	public void OnBeforeSerialize()
	{
		try
		{
			if (AudioClips.Any())
			{
				var clips = new List<AudioClip>();
				var types = new List<SoundType>();
				var ids = new List<ulong>();
				foreach (var c in AudioClips)
				{
					clips.Add(c.Value.Clip);
					types.Add(c.Value.SoundType);
					ids.Add((ulong)c.Key);
				}
				_audioClips = clips.ToArray();
				_clipTypes = types.ToArray();
				_clipIds = ids.ToArray();
			}
		}
		catch { }
	}
}

/// <summary>
/// Pair of clip and its intended routing type.
/// </summary>
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