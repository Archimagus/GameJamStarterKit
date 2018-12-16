using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
	public SceneField MainMenuScene;
	public SceneField GameScene;

	public void LoadMainMenuScene()
	{
		SceneManager.LoadScene(MainMenuScene);
	}
	public void LoadGameScene()
	{
		SceneManager.LoadScene(GameScene);
	}

	public void QuitGame()
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
