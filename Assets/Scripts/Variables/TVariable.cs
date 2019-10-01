using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 0649
[System.Serializable]
public abstract class TVariable : ScriptableObject
{
	public abstract void Reset();
}
public abstract class TVariable<T> : TVariable
{
	[SerializeField]
	protected T Value;
	private T _currentValue;
	public event EventHandler<ReferenceChangedEventHandler<T>> Changed;

	[Tooltip("Reset this variable to default when this scene is loaded. Leave blank to only reset when a new game is loaded")]
	[SerializeField] private SceneField _resetOnScene;

	public T CurrentValue
	{
		get
		{
			return _currentValue;
		}
		set
		{
			if (value?.Equals(_currentValue)??false)
				return;
			SetValue(value);
		}
	}
	private void Awake()
	{
		if(_resetOnScene != null)
			SceneManager.activeSceneChanged += sceneManager_activeSceneChanged;
	}


	protected virtual void OnValidate()
	{
		Reset();
	}
	private void sceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
	{
		if (newScene.name == _resetOnScene?.SceneName)
			Reset();
	}
	public override void Reset()
	{
		CurrentValue = Value;
	}
	public static implicit operator T(TVariable<T> i) { return i.CurrentValue; }

	protected virtual void SetValue(T val)
	{
		var oldValue = _currentValue;
		_currentValue = val;
		Changed?.Invoke(this, new ReferenceChangedEventHandler<T>(oldValue, _currentValue));
	}
}

public class ClampableVarible<T> : TVariable<T>
{
	[SerializeField] [HideInInspector] private bool _clamp = false;
	[SerializeField] [HideInInspector] private T _min;
	[SerializeField] [HideInInspector] private T _max;

	protected override void OnValidate()
	{
		base.OnValidate();
		dynamic val = Value;

		if (_min > val)
			_min = val;
		if (_max < val)
			_max = val;
	}

	protected override void SetValue(T val)
	{
		dynamic v = val;

		if (_clamp)
		{
			if (v > _max)
				val = _max;
			if (v < _min)
				val = _min;
		}

		base.SetValue(val);
	}
}

[System.Serializable]
public abstract class VarReference
{

}
[System.Serializable]
public abstract class TReference<T, V> : VarReference where V : TVariable<T>
{
	[SerializeField] private bool UseConstant = true;
	[SerializeField] private T ConstantValue;
	[SerializeField] private V Variable;
	private bool _changeRegistered;
	event Action<ReferenceChangedEventHandler<T>> _changed;
	public event Action<ReferenceChangedEventHandler<T>> Changed
	{
		add
		{
			_changed += value;
			if (_changeRegistered == false && UseConstant == false)
			{
				_changeRegistered = true;
				Variable.Changed += (s, e) => _changed?.Invoke(e);
			}
		}
		remove
		{
			_changed -= value;
		}
	}

	public TReference(T initial)
	{
		ConstantValue = initial;
	}
	public T Value
	{
		get
		{
			validate();
			return (UseConstant || Variable == null) ? ConstantValue : Variable.CurrentValue;
		}
		set
		{
			validate();
			if (value.Equals(Value))
				return;

			var oldValue = Value;
			if (UseConstant)
				ConstantValue = value;
			else
				Variable.CurrentValue = value;

			_changed?.Invoke(new ReferenceChangedEventHandler<T>(oldValue, Value));
		}
	}

	private void validate()
	{
		if (UseConstant == false)
		{
			if (Variable == null)
			{
				Debug.LogWarning("No Variable registered");
				return;
			}
		}
	}
	public static implicit operator T(TReference<T, V> i) { return i.Value; }
}

public class ReferenceChangedEventHandler<T> : EventArgs
{
	public T OldValue;
	public T NewValue;
	internal ReferenceChangedEventHandler(T oldValue, T newValue)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}