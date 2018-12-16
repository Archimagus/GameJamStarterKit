using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

public enum SoundType
{
	Default,
	SoundEffect,
	Ambience,
	Music,
	Interface,
	Dialogue
}

public static class AudioManager
{
	public static AudioMixer Mixer;
	public static AudioMixerGroup MasterMixerGroup;
	public static AudioMixerGroup EffectsMixerGroup;
	public static AudioMixerGroup AmbienceMixerGroup;
	public static AudioMixerGroup InterfaceMixerGroup;
	public static AudioMixerGroup MusicMixerGroup;
	public static AudioMixerGroup DialogueMixerGroup;
	public static AudioDatabase AudioDatabase;

	public static AudioClip AudioMissing;

	private static readonly Queue<AudioSource> _audioSources = new Queue<AudioSource>();

	private static GameObject _gameObject;
	private static CrossFader _crossfader;

	private static AudioSource _currentMusicSource;
	private static AudioSource _nextMusicSource;

	private static bool _fading = false;
	private static float _fadeTime = 1;
	private static float _fadeStartTime = 0;

	static AudioManager()
	{
		_gameObject = new GameObject();
		_gameObject.name = "AudioManager";

		Object.DontDestroyOnLoad(_gameObject);

		LoadResources();

		_currentMusicSource = _gameObject.AddComponent<AudioSource>();
		_currentMusicSource.loop = true;
		_currentMusicSource.outputAudioMixerGroup = MusicMixerGroup;

		_nextMusicSource = _gameObject.AddComponent<AudioSource>();
		_nextMusicSource.loop = true;
		_nextMusicSource.outputAudioMixerGroup = MusicMixerGroup;

		_crossfader = _gameObject.AddComponent<CrossFader>();
		_crossfader.StartCoroutine(CrossfadeMusic());
	}

	public static float MasterVolume
	{
		get { return PlayerPrefs.GetFloat("masterVolume", 0.75f).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("masterVolume", vol);
			Mixer.SetFloat("masterVolume", vol);
		}
	}


	public static float SoundEffectsVolume
	{
		get { return PlayerPrefs.GetFloat("effectsVolume", 1.0f).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("effectsVolume", vol);
			Mixer.SetFloat("effectsVolume", vol);
		}
	}
	public static float AmbienceVolume
	{
		get { return PlayerPrefs.GetFloat("ambianceVolume", 1.0f).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("ambianceVolume", vol);
			Mixer.SetFloat("ambianceVolume", vol);
		}
	}
	public static float MusicVolume
	{
		get { return PlayerPrefs.GetFloat("musicVolume", 0.5f).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("musicVolume", vol);
			Mixer.SetFloat("musicVolume", vol);
		}
	}
	public static float InterfaceVolume
	{
		get { return PlayerPrefs.GetFloat("interfaceVolume", 1.0f).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("interfaceVolume", vol);
			Mixer.SetFloat("interfaceVolume", vol);
		}
	}
	public static float DialogueVolume
	{
		get { return PlayerPrefs.GetFloat("dialogueVolume", 1.0f).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("dialogueVolume", vol);
			Mixer.SetFloat("dialogueVolume", vol);
		}
	}

	private static void LoadResources()
	{
		Mixer = Resources.Load<AudioMixer>("Master");
		MasterMixerGroup = Mixer.FindMatchingGroups("Master").First();
		MusicMixerGroup = Mixer.FindMatchingGroups("Music").First();
		EffectsMixerGroup = Mixer.FindMatchingGroups("Effects").First();
		AmbienceMixerGroup = Mixer.FindMatchingGroups("Ambiance").First();
		InterfaceMixerGroup = Mixer.FindMatchingGroups("Interface").First();
		DialogueMixerGroup = Mixer.FindMatchingGroups("Dialogue").First();
		
		AudioMissing = Resources.Load<AudioClip>("audiomissing");
		AudioDatabase = Resources.Load<AudioDatabase>("AudioDatabase");
	}

	public static void ResetVolumes()
	{
		Mixer.SetFloat("masterVolume", MasterVolume.todB());
		Mixer.SetFloat("effectsVolume", SoundEffectsVolume.todB());
		Mixer.SetFloat("ambianceVolume", AmbienceVolume.todB());
		Mixer.SetFloat("interfaceVolume", InterfaceVolume.todB());
		Mixer.SetFloat("musicVolume", MusicVolume.todB());
		Mixer.SetFloat("dialogueVolume", DialogueVolume.todB());
	}
	public static void PlayMusic(AudioClips clip, float fadeTime = 1, bool sync = false)
	{
		AudioDatabase.AudioClips.TryGetValue(clip, out AudioClipData cd);
		PlayMusic(cd?.Clip, fadeTime);
	}
	public static void PlayMusic(AudioClip clip, float fadeTime = 1, bool sync = false)
	{
		if (clip == null)
		{
			Debug.Log("PlayMusic is trying to play a null audio clip.");
			clip = AudioMissing;
		}

		if (_currentMusicSource.isPlaying)
		{
			_nextMusicSource.clip = clip;
			_nextMusicSource.Play();

			if (sync)
			{
				_nextMusicSource.timeSamples = _currentMusicSource.timeSamples;
			}

			_fadeTime = fadeTime;
			_fadeStartTime = Time.time;
			_fading = true;
		}
		else
		{
			_currentMusicSource.clip = clip;
			_currentMusicSource.Play();
		}
	}

	private static IEnumerator CrossfadeMusic ()
	{
		while (true)
		{
			if (_fading)
			{
				float crossfadeValue = Mathf.Lerp(0, 1, (Time.time - _fadeStartTime) / _fadeTime);
				_nextMusicSource.volume = crossfadeValue;
				_currentMusicSource.volume = (1 - crossfadeValue);

				if (Time.time - _fadeStartTime > _fadeTime)
				{
					_fading = false;

					_nextMusicSource.volume = 1;
					_currentMusicSource.volume = 0;
					_currentMusicSource.Stop();

					AudioSource tmp = _currentMusicSource;
					_currentMusicSource = _nextMusicSource;
					_nextMusicSource = tmp;
				}
			}
			yield return null;
		}
	}

	public static void PlaySound(this MonoBehaviour sfxSource, AudioClips clip,
		SoundType type = SoundType.Default, Vector3? location = null)
	{
		AudioDatabase.AudioClips.TryGetValue(clip, out AudioClipData cd);
		PlaySound(sfxSource, cd?.Clip, type==SoundType.Default?cd?.SoundType??type:type, location);
	}
	public static void PlaySound(this MonoBehaviour sfxSource, AudioClip clip,
		SoundType type = SoundType.Default, Vector3? location = null)
	{
		if (sfxSource == null)
		{
			sfxSource = _crossfader;
		}
		if (clip == null)
		{
			Debug.Log(sfxSource.name + " trying to play a null audio clip.");
			clip = AudioMissing;
		}
		AudioSource source;
		if (_audioSources.Count > 0)
		{
			source = _audioSources.Dequeue();
		}
		else
		{
			var go = new GameObject("soundEffectDummy");
			source = go.AddComponent<AudioSource>();
			Object.DontDestroyOnLoad(go);
		}
		source.gameObject.SetActive(true);
		if (location != null)
		{
			source.transform.SetParent(_gameObject.transform);
			source.transform.position = location.Value;
		}
		else
		{
			source.transform.SetParent(sfxSource.transform);
			source.transform.localPosition = Vector3.zero;
		}
		if (type == SoundType.Default)
			AudioDatabase.ClipTypes.TryGetValue(clip, out type);
		switch (type)
		{
			case SoundType.SoundEffect:
				source.outputAudioMixerGroup = EffectsMixerGroup;
				break;
			case SoundType.Ambience:
				source.outputAudioMixerGroup = AmbienceMixerGroup;
				break;
			case SoundType.Music:
				source.outputAudioMixerGroup = MusicMixerGroup;
				break;
			case SoundType.Interface:
				source.outputAudioMixerGroup = InterfaceMixerGroup;
				break;
			case SoundType.Dialogue:
				source.outputAudioMixerGroup = DialogueMixerGroup;
				break;
			default:
				source.outputAudioMixerGroup = MasterMixerGroup;
				break;
		}
		source.clip = clip;
		source.Play();
		sfxSource.StartCoroutine(requeueSource(source));
	}

	private static IEnumerator requeueSource(AudioSource source)
	{
		yield return new WaitForSeconds(source.clip.length);
		source.transform.SetParent(_gameObject.transform);
		source.gameObject.SetActive(false);
		_audioSources.Enqueue(source);
	}

	private static float todB(this float lin)
	{
		if (lin <= float.Epsilon)
			return -80;
		return Mathf.Log(lin,3) * 20;
	}

	private static float toLin(this float db)
	{
		return Mathf.Pow(3, db / 20);
	}

}

class CrossFader : MonoBehaviour
{
	private void Start()
	{
		AudioManager.ResetVolumes();
	}
}
