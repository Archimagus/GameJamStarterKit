using UnityEngine;

[CreateAssetMenu(menuName ="Variables/Int")]
public class IntVariable : ClampableVarible<int>
{
}

[System.Serializable]
public class IntReference : TReference<int, IntVariable>
{
	public IntReference(int initial) : base(initial)
	{
	}
}
