using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Float")]
public class FloatVariable : ClampableVarible<float>
{
}
[System.Serializable]
public class FloatReference : TReference<float, FloatVariable>
{
	public FloatReference(float initial):base(initial)
	{
	}
}