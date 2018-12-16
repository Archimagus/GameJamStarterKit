using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public GameObject PausePanel;
	public static PauseMenu Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}
}
