using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Simple stack-based menu manager. Opens/Closes GameObjects in LIFO order and pauses gameplay while any menus are open.
/// </summary>
public class MenuStack : ScriptableObject
{
	[Tooltip("Global time controller used to pause when menus are open.")]
	[SerializeField] private GameTime _gameTime;
	[SerializeField] private Stack<MenuStackItem> _menus = new Stack<MenuStackItem>();

	public void OpenMenu(GameObject menu)
	{
		OpenMenu(menu, true);
	}
	/// <summary>
	/// Pushes a menu onto the stack and sets it active. Optionally disables the current top.
	/// </summary>
	public void OpenMenu(GameObject menu, bool diableCurrent)
	{
		if (_menus.Any() && diableCurrent)
			_menus.Peek().MenuItem.SetActive(false);

		_menus.Push(new MenuStackItem(menu, diableCurrent));
		menu.SetActive(true);
		_gameTime.MenuPause = true;
	}
	/// <summary>
	/// Pops menus until an independent menu is encountered. Outputs the number of closed menus.
	/// </summary>
	public void CloseMenu(out int menusClosed)
	{
		menusClosed = 0;
		if (!_menus.Any())
			return;

		MenuStackItem menu;
		do
		{
			menu = _menus.Pop();
			menu.MenuItem.SetActive(false);
			menusClosed++;
			if (!_menus.Any())
			{
				_gameTime.MenuPause = false;
			}
			else
			{
				_menus.Peek().MenuItem.SetActive(true);
			}
		} while (!menu.Independent);
	}
	/// <summary>
	/// Closes a single menu or clears the stack if none are independent.
	/// </summary>
	public void CloseMenu()
	{
		CloseMenu(out _);
	}
	private class MenuStackItem
	{
		public GameObject MenuItem;
		public bool Independent;
		public MenuStackItem(GameObject menuItem, bool independent)
		{
			MenuItem = menuItem;
			Independent = independent;
		}
	}
}