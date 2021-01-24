using UnityEngine;

[CreateAssetMenu(menuName = "Variables/String")]
public class StringVariable : TVariable<string>
{
}
[System.Serializable]
public class StringReference : TReference<string, StringVariable>
{
	public StringReference(string initial) : base(initial)
	{
	}
}
