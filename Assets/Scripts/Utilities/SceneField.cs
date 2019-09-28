using UnityEngine;
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