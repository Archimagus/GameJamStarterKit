using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListner : MonoBehaviour
{
	[Tooltip("The primary event to listen to")]
	public GameEvent Event;
	[Tooltip("Additional events to listen to")]
	public GameEvent[] Events;
	[Tooltip("The response to the event")]
	public DataEvent Response;
	private DataVariable _dataVariable;

	private void Awake()
	{
		_dataVariable = ScriptableObject.CreateInstance<DataVariable>();
		_dataVariable.name = name + "Data";
	}
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
	private void OnDisable()
	{
		Event?.UnregisterListner(this);
		foreach (var e in Events)
		{
			e.UnregisterListner(this);
		}
	}

	public void OnEventRaised()
	{
		if (Response == null)
			Debug.LogWarning("No Response", this);
		_dataVariable.Data = null;
		Response?.Invoke(_dataVariable);
	}
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