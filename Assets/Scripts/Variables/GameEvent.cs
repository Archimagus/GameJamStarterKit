using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	private List<GameEventListner> listners = new List<GameEventListner>();
	private void Awake()
	{
		listners.Clear();
	}
	public void Raise(EventData data)
	{
		for (int i = listners.Count - 1; i >= 0; i--)
		{
			listners[i].OnEventRaised(data);
		}
	}
	public void Raise()
	{
		for (int i = listners.Count-1; i>=0; i--)
		{
			listners[i].OnEventRaised();
		}
	}

	public void RegisterListner(GameEventListner listner)
	{
		listners.Add(listner);
	}
	public void UnregisterListner(GameEventListner listner)
	{
		listners.Remove(listner);
	}
}