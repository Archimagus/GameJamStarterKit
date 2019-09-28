using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Bool")]
public class BoolVariable : TVariable<bool>
{
}
[System.Serializable]
public class BoolReference : TReference<bool, BoolVariable>
{
	public BoolReference(bool initial) : base(initial)
	{
	}
}
