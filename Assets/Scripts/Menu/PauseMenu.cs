using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
	[Tooltip("Root panel that is shown when the game is paused.")]
	public GameObject PausePanel;
	public static PauseMenu Instance { get; private set; }
	/// <summary>
	/// Ensures a single instance for easy access by other systems.
	/// </summary>
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}
	/// <summary>
	/// Maintains UI selection for controller/keyboard when paused.
	/// </summary>
	private void Update()
	{
		if (PausePanel.activeInHierarchy)
		{
			var es = EventSystem.current;
			if ((Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f && es.currentSelectedGameObject == null) ||
				(es.currentSelectedGameObject != null && es.currentSelectedGameObject.activeInHierarchy == false))
			{

				es.SetSelectedGameObject(GameObject.Find("ResumeButton").gameObject);
			}
		}
	}
}
