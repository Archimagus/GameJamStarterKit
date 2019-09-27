using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Menu Stuff")]
	[SerializeField] private Button _quitButton = null;
	[SerializeField] private GameObject _mainMenuPanel = null;
	[SerializeField] private GameObject _settingPanel = null;

	private GameTime _gameTime;
	private GameData _gamedata;
	private MenuStack _menuStack;

	private void Awake()
	{
		_gameTime = Resources.Load<GameTime>("GameTime");
		if (_gameTime == null)
			Debug.LogError("GameTime not found");
		_menuStack = Resources.Load<MenuStack>("MenuStack");
		if (_menuStack == null)
			Debug.LogError("MenuStack not found");
		_gamedata = Resources.Load<GameData>("GameData");
		if (_gamedata == null)
			Debug.LogError("GameData not found");

		if (Application.platform == RuntimePlatform.WebGLPlayer)
			_quitButton.gameObject.SetActive(false);
		if (!Input.mousePresent)
		{
			EventSystem.current.SetSelectedGameObject(GameObject.Find("NewGameButton").gameObject);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_menuStack.CloseMenu(out int closed);
			if(closed == 0)
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
