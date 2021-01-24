using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
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

	public static bool IsInitialized => (AudioMissing != null && AudioDatabase != null && Mixer != null);

	static AudioManager()
	{
		_gameObject = new GameObject();
		_gameObject.name = "AudioManager";

		Object.DontDestroyOnLoad(_gameObject);

		Addressables.LoadAssetAsync<AudioClip>("audiomissing").Completed += (am) =>
		{
			if (am.Status == AsyncOperationStatus.Failed)
				Debug.LogError("Cant Load audiomissing");
			if (am.IsDone)
				AudioMissing = am.Result; 
		};
		Addressables.LoadAssetAsync<AudioDatabase>("AudioDatabase").Completed += (ad) =>
		{
			if (ad.Status == AsyncOperationStatus.Failed)
				Debug.LogError("Cant Load AudioDatabase");
			if (ad.IsDone) 
				AudioDatabase = ad.Result; 
		};

		var mrEvent = Addressables.LoadAssetAsync<AudioMixer>("Master.mixer");

		_currentMusicSource = _gameObject.AddComponent<AudioSource>();
		_currentMusicSource.loop = true;
		_nextMusicSource = _gameObject.AddComponent<AudioSource>();
		_nextMusicSource.loop = true;
		_crossfader = _gameObject.AddComponent<CrossFader>();
		_crossfader.StartCoroutine(CrossfadeMusic());


		mrEvent.Completed += (mr) =>
		{
			if (mr.Status == AsyncOperationStatus.Failed)
				Debug.LogError("Cant Load Master.mixer");
			if (mr.IsDone)
			{
				var mixer = mr.Result;
				MasterMixerGroup = mixer.FindMatchingGroups("Master").First();
				MusicMixerGroup = mixer.FindMatchingGroups("Music").First();
				EffectsMixerGroup = mixer.FindMatchingGroups("Effects").First();
				AmbienceMixerGroup = mixer.FindMatchingGroups("Ambiance").First();
				InterfaceMixerGroup = mixer.FindMatchingGroups("Interface").First();
				DialogueMixerGroup = mixer.FindMatchingGroups("Dialogue").First();

				_currentMusicSource.outputAudioMixerGroup = MusicMixerGroup;
				_nextMusicSource.outputAudioMixerGroup = MusicMixerGroup;

				Mixer = mixer;
				ResetVolumes();
			}
		};

	}

	public static float MasterVolume
	{
		get { return PlayerPrefs.GetFloat("masterVolume", 0.5f.todB()).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("masterVolume", vol);
			Mixer.SetFloat("masterVolume", vol);
		}
	}


	public static float SoundEffectsVolume
	{
		get { return PlayerPrefs.GetFloat("effectsVolume", 0.5f.todB()).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("effectsVolume", vol);
			Mixer.SetFloat("effectsVolume", vol);
		}
	}
	public static float AmbienceVolume
	{
		get { return PlayerPrefs.GetFloat("ambianceVolume", 0.5f.todB()).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("ambianceVolume", vol);
			Mixer.SetFloat("ambianceVolume", vol);
		}
	}
	public static float MusicVolume
	{
		get { return PlayerPrefs.GetFloat("musicVolume", 0.5f.todB()).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("musicVolume", vol);
			Mixer.SetFloat("musicVolume", vol);
		}
	}
	public static float InterfaceVolume
	{
		get { return PlayerPrefs.GetFloat("interfaceVolume", 0.5f.todB()).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("interfaceVolume", vol);
			Mixer.SetFloat("interfaceVolume", vol);
		}
	}
	public static float DialogueVolume
	{
		get { return PlayerPrefs.GetFloat("dialogueVolume", 0.5f.todB()).toLin(); }
		set
		{
			var vol = value.todB();
			PlayerPrefs.SetFloat("dialogueVolume", vol);
			Mixer.SetFloat("dialogueVolume", vol);
		}
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
	private static IEnumerator queueAction(Action action)
	{
		while(!IsInitialized)
			yield return null;
		action();
	}
	public static void PlayMusic(AudioClips clip, float fadeTime = 1, bool sync = false)
	{
		AudioClipData cd = AudioDatabase[clip];
		PlayMusic(cd?.Clip, fadeTime);
	}
	public static void PlayMusic(AudioClip clip, float fadeTime = 1, bool sync = false)
	{
		if (!IsInitialized)
		{
			_crossfader.StartCoroutine(queueAction(() => PlayMusic(clip, fadeTime, sync)));
			return;
		}

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
		AudioClipData cd = AudioDatabase[clip];
		PlaySound(sfxSource, cd?.Clip, type==SoundType.Default?cd?.SoundType??type:type, location);
	}
	public static void PlaySound(this MonoBehaviour sfxSource, AudioClip clip,
		SoundType type = SoundType.Default, Vector3? location = null)
	{
		if (!IsInitialized)
		{
			_crossfader.StartCoroutine(queueAction(() => PlaySound(sfxSource, clip, type, location)));
			return;
		}

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
			type = AudioDatabase[clip];

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
}
