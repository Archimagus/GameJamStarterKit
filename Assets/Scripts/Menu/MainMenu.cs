using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Game Stuff")]
	[SerializeField] private SceneField _gamePlayScene = null;

	[Header("Menu Stuff")]
	[SerializeField] private Button _quitButton = null;
	[SerializeField] private GameObject _mainMenuPanel = null;
	[SerializeField] private GameObject _settignsPanel = null;

	private GameTime _gameTime;
	private MenuStack _menuStack;

	private void Awake()
	{
		_gameTime = Resources.Load<GameTime>("GameTime");
		if (_gameTime == null)
			Debug.LogError("GameTime not found");
		_menuStack = Resources.Load<MenuStack>("MenuStack");
		if (_menuStack == null)
			Debug.LogError("MenuStack not found");

		_menuStack.OpenMenu(_mainMenuPanel);
		if (Application.platform == RuntimePlatform.WebGLPlayer)
			_quitButton.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_menuStack.CloseMenu();
			if (!_gameTime.MenuPause)
				Quit();
		}
	}

	public void ContinueGame()
	{
		if (_gamePlayScene != null)
		{
			_menuStack.CloseMenu();
			SceneManager.LoadScene(_gamePlayScene);
		}
	}
	public void NewGame()
	{
		if (_gamePlayScene != null)
		{
			_menuStack.CloseMenu();
			SceneManager.LoadScene(_gamePlayScene);
		}
	}
	public void ShowSettings()
	{
		_menuStack.OpenMenu(_settignsPanel);
	}
	public void Quit()
	{
		if (Application.isEditor)
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}
		else
		{
			Application.Quit();
		}
	}
}
