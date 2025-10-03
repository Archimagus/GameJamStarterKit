using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Menu Stuff")]
	[Tooltip("Hide in WebGL; quits the application on desktop.")]
	[SerializeField] private Button _quitButton = null;
	[Tooltip("The panel containing the main menu buttons.")]
	[SerializeField] private GameObject _mainMenuPanel = null;
	[Tooltip("The panel containing the settings UI.")]
	[SerializeField] private GameObject _settingPanel = null;

	[Header("Scriptable Objects")]
	[Tooltip("Global time controller used to manage menu pause state.")]
	[SerializeField] private GameTime _gameTime;
	[Tooltip("Holds references and helpers for scene loading.")]
	[SerializeField] private GameData _gamedata;
	[Tooltip("Shared menu stack asset used to open/close menus.")]
	[SerializeField] private MenuStack _menuStack;

	/// <summary>
	/// Validates references and applies platform-specific UI defaults.
	/// </summary>
	private void Awake()
	{
		if (_gameTime == null)
			Debug.LogError("GameTime not found");
		if (_menuStack == null)
			Debug.LogError("MenuStack not found");
		if (_gamedata == null)
			Debug.LogError("GameData not found");

		if (Application.platform == RuntimePlatform.WebGLPlayer)
			_quitButton.gameObject.SetActive(false);
		if (!Input.mousePresent)
		{
			EventSystem.current.SetSelectedGameObject(GameObject.Find("NewGameButton").gameObject);
		}
	}

	/// <summary>
	/// Handles back/escape to close settings or quit, and maintains UI selection for non-mouse input.
	/// </summary>
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_menuStack.CloseMenu(out int closed);
			if (closed == 0)
				_gamedata.QuitGame();
		}
		var es = EventSystem.current;
		if (_mainMenuPanel.activeInHierarchy &&
			((Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f && es.currentSelectedGameObject == null) ||
				(es.currentSelectedGameObject != null && es.currentSelectedGameObject.activeInHierarchy == false)))
		{
			es.SetSelectedGameObject(GameObject.Find("NewGameButton").gameObject);
		}
		_mainMenuPanel.SetActive(!_settingPanel.activeInHierarchy);
	}
}
