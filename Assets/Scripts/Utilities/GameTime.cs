
using UnityEngine;

public class GameTime : ScriptableObject
{
	private float _timeScale;
	private bool _pause;
	private bool _menuPause;
	private void Awake()
	{
		init();
	}
	private void OnValidate()
	{
		init();
	}
	private void init()
	{
		_timeScale = 1f;
		_pause = false;
		_menuPause = false;
	}
	public float TimeScale
	{
		get
		{
			return _timeScale;
		}
		set
		{
			_timeScale = value;
			if (!(Pause || MenuPause))
			{
				setTimeScale(value);
			}
		}
	}
	public bool Pause
	{
		get { return _pause; }
		set
		{
			_pause = value;
			if (value)
			{
				setTimeScale(0);
			}
			else
			{
				if (!MenuPause)
				{
					setTimeScale(TimeScale);
				}
			}
		}
	}
	public bool MenuPause
	{
		get
		{
			return _menuPause;
		}
		set
		{
			_menuPause = value;
			if (value)
			{
				setTimeScale(0);
			}
			else
			{
				if (!Pause)
				{
					setTimeScale(TimeScale);
				}
			}
		}
	}
	private void setTimeScale(float value)
	{
		Time.timeScale = value;
		Time.fixedDeltaTime = value * 0.02f;
	}
}