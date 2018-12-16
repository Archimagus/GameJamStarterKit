using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Menu Stuff")]
	[SerializeField] private Button _quitButton = null;

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
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_menuStack.CloseMenu(out int closed);
			if(closed == 0)
				_gamedata.QuitGame();
		}
	}
}
