using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsPanel : MonoBehaviour
{
	[SerializeField] private Slider _masterSlider = null;
	[SerializeField] private Slider _musicSlider = null;
	[SerializeField] private Slider _soundEffectsSlider = null;
	[SerializeField] private Slider _ambienceSlider = null;
	[SerializeField] private Slider _dialogueSlider = null;
	[SerializeField] private Slider _uISlider = null;

	private float _musicVolume;
	private float _soundEffectsVolume;
	private float _ambienceVolume;
	private float _dialogueVolume;
	private float _uIVolume;

	public float MasterVolume
	{
		get
		{
			return AudioManager.MasterVolume;
		}
		set
		{
			AudioManager.MasterVolume = value;
		}
	}
	public float MusicVolume
	{
		get
		{
			return AudioManager.MusicVolume;
		}
		set
		{
			AudioManager.MusicVolume = value;
		}
	}
	public float SoundEffectsVolume
	{
		get
		{
			return AudioManager.SoundEffectsVolume;
		}
		set
		{
			AudioManager.SoundEffectsVolume = value;
		}
	}
	public float AmbienceVolume
	{
		get
		{
			return AudioManager.AmbienceVolume;
		}
		set
		{
			AudioManager.AmbienceVolume = value;
		}
	}
	public float DialogueVolume
	{
		get
		{
			return AudioManager.DialogueVolume;
		}
		set
		{
			AudioManager.DialogueVolume = value;
		}
	}
	public float UIVolume
	{
		get
		{
			return AudioManager.InterfaceVolume;
		}
		set
		{
			AudioManager.InterfaceVolume = value;
		}
	}
	void OnEnable()
	{
		_masterSlider.value = AudioManager.MasterVolume;
		_musicSlider.value = AudioManager.MusicVolume;
		_soundEffectsSlider.value = AudioManager.SoundEffectsVolume;
		_uISlider.value = AudioManager.InterfaceVolume;
		_ambienceSlider.value = AudioManager.AmbienceVolume;
		_dialogueSlider.value = AudioManager.DialogueVolume;
	}
}
