using UnityEngine;
/// <summary>
/// Serializable reference to a scene by asset with implicit conversion to scene name string.
/// </summary>
[System.Serializable]
public class SceneField
{
	[SerializeField]
	private Object _sceneAsset;
	[SerializeField]
	private string _sceneName = "";
	public string SceneName
	{
		get { return _sceneName; }
	}
	// makes it work with the existing Unity methods (LoadLevel/LoadScene)
	public static implicit operator string(SceneField sceneField)
	{
		return sceneField.SceneName;
	}
}