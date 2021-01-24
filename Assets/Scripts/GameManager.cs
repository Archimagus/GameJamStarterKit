using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameTime _gameTime;
	[SerializeField] private MenuStack _menuStack;
	public static GameManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		if (_gameTime == null)
			Debug.LogError("GameTime not found");

		if (_menuStack == null)
			Debug.LogError("MenuStack not found");
	}
	
	private void Update()
	{
		if (Input.GetButtonDown("Menu"))
		{
			if (_gameTime.MenuPause)
				_menuStack.CloseMenu();
			else
				_menuStack.OpenMenu(PauseMenu.Instance.PausePanel);
		}
	}
	public void QuitToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}