using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central runtime coordinator for pause menu toggling and scene transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
	[Tooltip("Global time controller used to pause/resume gameplay.")]
	[SerializeField] private GameTime _gameTime;
	[Tooltip("Shared menu stack asset used to open/close menus.")]
	[SerializeField] private MenuStack _menuStack;
	public static GameManager Instance { get; private set; }
	/// <summary>
	/// Ensures a single instance and validates required references.
	/// </summary>
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

	/// <summary>
	/// Toggles the pause menu when the configured "Menu" input is pressed.
	/// </summary>
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
	/// <summary>
	/// Loads the main menu scene by name.
	/// </summary>
	public void QuitToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}