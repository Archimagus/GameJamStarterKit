using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
	[Tooltip("Root panel for settings content to be opened on the menu stack.")]
	[SerializeField] private GameObject _mainPanel = null;
	[Tooltip("Shared menu stack asset used to open the settings panel.")]
	[SerializeField] private MenuStack _menuStack = null;

	/// <summary>
	/// Validates required references.
	/// </summary>
	private void Awake()
	{
		if (_menuStack == null)
			Debug.LogError("MenuStack not found");
	}
	/// <summary>
	/// Opens the settings panel on enable without disabling the previous menu.
	/// </summary>
	private void OnEnable()
	{
		_menuStack.OpenMenu(_mainPanel, false);
	}
}
