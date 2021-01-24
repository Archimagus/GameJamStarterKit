using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
	[SerializeField] private GameObject _mainPanel = null;
	[SerializeField] private MenuStack _menuStack = null;

	private void Awake()
	{
		if (_menuStack == null)
			Debug.LogError("MenuStack not found");
	}
	private void OnEnable()
	{
		_menuStack.OpenMenu(_mainPanel, false);
	}
}
