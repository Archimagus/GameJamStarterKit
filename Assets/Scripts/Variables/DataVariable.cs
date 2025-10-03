using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Wrapper ScriptableObject for passing <see cref="EventData"/> through UnityEvents.
/// </summary>
public class DataVariable : ScriptableObject
{
	public EventData Data;

	public int Count { get { return Data.Count; } }
	public EventDataElement this[int index]
	{
		get { return Data[index]; }
		set { Data[index] = value; }
	}
	public EventDataElement this[string key]
	{
		get { return Data[key]; }
	}
	public IEnumerator GetEnumerator()
	{
		return Data.GetEnumerator();
	}
}
/// <summary>
/// Serializable payload map used when raising <see cref="GameEvent"/>s.
/// Stores items in a dictionary and serializes via a backing list for the editor.
/// </summary>
[System.Serializable]
public class EventData : IEnumerable, ISerializationCallbackReceiver
{
	[SerializeField] private List<EventDataElement> _data = new List<EventDataElement>();
	public Dictionary<string, EventDataElement> Data = new Dictionary<string, EventDataElement>();

	public int Count { get { return Data.Count; } }
	public EventDataElement this[int index]
	{
		get { return _data[index]; }
		set { _data[index] = value; }
	}
	public EventDataElement this[string key]
	{
		get
		{
			Data.TryGetValue(key, out var value);
			return value;
		}
	}
	public IEnumerator GetEnumerator()
	{
		return _data.GetEnumerator();
	}

	public void OnBeforeSerialize()
	{
		_data.Clear();
		foreach (var item in Data.Values)
		{
			_data.Add(item);
		}
	}

	public void OnAfterDeserialize()
	{
		Data.Clear();
		foreach (var item in _data)
		{
			Data[item.Key] = item;
		}
	}
}


/// <summary>
/// Single typed entry held by <see cref="EventData"/>.
/// Supports optional int, float, or string values.
/// </summary>
[System.Serializable]
public class EventDataElement
{
	public string Key;
	public int? IntValue;
	public float? FloatValue;
	public string StringValue;
	public EventDataElement()
	{

	}
	public EventDataElement(string key, int? intValue = null, float? floatValue = null, string stringValue = null)
	{
		Key = key;
		IntValue = intValue;
		FloatValue = floatValue;
		StringValue = stringValue;
	}
}