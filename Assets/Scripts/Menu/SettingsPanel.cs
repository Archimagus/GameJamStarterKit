using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
	[SerializeField] private GameObject _mainPanel = null;
	private MenuStack _menuStack;

	private void Awake()
	{
		_menuStack = Resources.Load<MenuStack>("MenuStack");
		if (_menuStack == null)
			Debug.LogError("MenuStack not found");
	}
	private void OnEnable()
	{
		_menuStack.OpenMenu(_mainPanel, false);
	}
}
