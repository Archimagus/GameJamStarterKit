using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable event that supports raising notifications with optional <see cref="EventData"/> payloads.
/// </summary>
[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	private List<GameEventListner> listners = new List<GameEventListner>();
	/// <summary>
	/// Clears listeners on domain reload to avoid stale references.
	/// </summary>
	private void Awake()
	{
		listners.Clear();
	}
	/// <summary>
	/// Raises the event and passes <paramref name="data"/> to listeners.
	/// </summary>
	public void Raise(EventData data)
	{
		for (int i = listners.Count - 1; i >= 0; i--)
		{
			listners[i].OnEventRaised(data);
		}
	}
	/// <summary>
	/// Raises the event without payload.
	/// </summary>
	public void Raise()
	{
		for (int i = listners.Count - 1; i >= 0; i--)
		{
			listners[i].OnEventRaised();
		}
	}

	/// <summary>
	/// Registers a listener.
	/// </summary>
	public void RegisterListner(GameEventListner listner)
	{
		listners.Add(listner);
	}
	/// <summary>
	/// Unregisters a listener.
	/// </summary>
	public void UnregisterListner(GameEventListner listner)
	{
		listners.Remove(listner);
	}
}