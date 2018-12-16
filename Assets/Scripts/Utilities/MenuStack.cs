using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MenuStack : ScriptableObject
{
	private GameTime _gameTime;
	private Stack<MenuStackItem> _menus = new Stack<MenuStackItem>();
	
	private void Awake()
	{
		_gameTime = Resources.Load<GameTime>("GameTime");
		if (_gameTime == null)
			Debug.LogError("GameTime not found");
	}
	public void OpenMenu(GameObject menu)
	{
		OpenMenu(menu, true);
	}
	public void OpenMenu(GameObject menu, bool diableCurrent)
	{
		if (_menus.Any() && diableCurrent)
			_menus.Peek().MenuItem.SetActive(false);

		_menus.Push(new MenuStackItem(menu, diableCurrent));
		menu.SetActive(true);
		_gameTime.MenuPause = true;
	}
	public void CloseMenu()
	{
		if (!_menus.Any())
			return;

		MenuStackItem menu;
		do
		{
			menu = _menus.Pop();
			menu.MenuItem.SetActive(false);
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