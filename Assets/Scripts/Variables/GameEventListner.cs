using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Component that subscribes to one or more <see cref="GameEvent"/>s and invokes a UnityEvent response.
/// Wraps payloads in a temporary <see cref="DataVariable"/> for convenient binding in the inspector.
/// </summary>
public class GameEventListner : MonoBehaviour
{
	[Tooltip("The primary event to listen to")]
	public GameEvent Event;
	[Tooltip("Additional events to listen to")]
	public GameEvent[] Events;
	[Tooltip("The response invoked when any configured event is raised")]
	public DataEvent Response;
	private DataVariable _dataVariable;

	/// <summary>
	/// Creates a temporary <see cref="DataVariable"/> used to pass payloads to the response.
	/// </summary>
	private void Awake()
	{
		_dataVariable = ScriptableObject.CreateInstance<DataVariable>();
		_dataVariable.name = name + "Data";
	}
	/// <summary>
	/// Registers for events when enabled.
	/// </summary>
	private void OnEnable()
	{
		if (Event == null)
			Debug.LogWarning("No Event", this);
		Event?.RegisterListner(this);
		foreach (var e in Events)
		{
			e.RegisterListner(this);
		}
	}
	/// <summary>
	/// Unregisters from events when disabled.
	/// </summary>
	private void OnDisable()
	{
		Event?.UnregisterListner(this);
		foreach (var e in Events)
		{
			e.UnregisterListner(this);
		}
	}

	/// <summary>
	/// Called by the event when raised without payload.
	/// </summary>
	public void OnEventRaised()
	{
		if (Response == null)
			Debug.LogWarning("No Response", this);
		_dataVariable.Data = null;
		Response?.Invoke(_dataVariable);
	}
	/// <summary>
	/// Called by the event when raised with payload.
	/// </summary>
	public void OnEventRaised(EventData data)
	{
		if (Response == null)
			Debug.LogWarning("No Response", this);
		_dataVariable.Data = data;
		Response?.Invoke(_dataVariable);
	}
}

[Serializable]
public class DataEvent : UnityEvent<DataVariable>
{

}