using UnityEngine;

public class GameSettingsPanel : MonoBehaviour
{
	public void ClearSettings()
	{
		PlayerPrefs.DeleteAll();
	}
}