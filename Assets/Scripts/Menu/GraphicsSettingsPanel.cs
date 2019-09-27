using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsPanel : MonoBehaviour
{
	[SerializeField] Toggle _settingsButtonPrefab = null;
	[SerializeField] ToggleGroup _resolutionOptionsHost = null;
	[SerializeField] ToggleGroup _qualityOptionsHost = null;
	[SerializeField] Toggle _fullScreenToggle = null;
	[SerializeField] Toggle _borderlessToggle = null;
	[SerializeField] Toggle _windowedToggle = null;
	private void OnEnable()
	{
		switch (Screen.fullScreenMode)
		{
			case FullScreenMode.ExclusiveFullScreen:
				_fullScreenToggle.isOn = true;
				break;
			case FullScreenMode.FullScreenWindow:
				_borderlessToggle.isOn = true;
				break;
			case FullScreenMode.MaximizedWindow:
				_windowedToggle.isOn = true;
				break;
			case FullScreenMode.Windowed:
				_windowedToggle.isOn = true;
				break;
		}
		_qualityOptionsHost.transform.DestroyChildren();
		for (var i = 0; i < QualitySettings.names.Length; i++)
		{
			int settingLevel = i;
			var setting = QualitySettings.names[i];

			var b = Instantiate(_settingsButtonPrefab, _qualityOptionsHost.transform);
			b.isOn = QualitySettings.GetQualityLevel() == settingLevel;
			b.group = _qualityOptionsHost;
			b.GetComponentInChildren<TextMeshProUGUI>().text = setting;
			b.onValueChanged.AddListener((state) => { if (state) { QualitySettings.SetQualityLevel(settingLevel); } });
		}
		updateResolutionsDialogue();
	}

	private void updateResolutionsDialogue()
	{
		var parent = _resolutionOptionsHost.transform;
		parent.DestroyChildren();

		void resolutionButtonClicked(Resolution r, bool state)
		{
			if (state)
				Screen.SetResolution(r.width, r.height, Screen.fullScreenMode, r.refreshRate);
		}
		var curRes = Screen.currentResolution;
		var b = Instantiate(_settingsButtonPrefab, parent);
		b.group = _resolutionOptionsHost;
		b.GetComponentInChildren<TextMeshProUGUI>().text = $"{curRes.width}X{curRes.height} {curRes.refreshRate}Hz";
		b.isOn = true;
		b.onValueChanged.AddListener((state) => resolutionButtonClicked(curRes, state));
		foreach (var res in Screen.resolutions)
		{
			var r = res;
			if (r.height == curRes.height && r.width == curRes.width && r.refreshRate == curRes.refreshRate)
				continue;
			b = Instantiate(_settingsButtonPrefab, parent);
			b.isOn = false;
			b.group = _resolutionOptionsHost;
			b.GetComponentInChildren<TextMeshProUGUI>().text = $"{r.width}X{r.height} {r.refreshRate}Hz";
			b.onValueChanged.AddListener((state) => resolutionButtonClicked(r, state));
		}
	}

	public bool FullScreen
	{
		get { return Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen; }
		set
		{
			if (value)
			{
				Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
				updateResolutionsDialogue();
			}
		}
	}
	public bool Borderless
	{
		get { return Screen.fullScreenMode == FullScreenMode.FullScreenWindow; }
		set
		{
			if (value)
			{
				Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
				updateResolutionsDialogue();
			}
		}
	}
	public bool Windowed
	{
		get { return Screen.fullScreenMode == FullScreenMode.Windowed; }
		set
		{
			if (value)
			{
				Screen.fullScreenMode = FullScreenMode.Windowed;
				updateResolutionsDialogue();
			}
		}
	}
}
