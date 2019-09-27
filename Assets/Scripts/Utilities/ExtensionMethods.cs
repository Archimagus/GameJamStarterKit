using UnityEngine;

static class ExtensionMethods
{
	public static void DestroyChildren(this Transform t)
	{
		while (t.childCount > 0)
		{
			var c = t.GetChild(0);
			c.SetParent(null);
			GameObject.Destroy(c.gameObject);
		}
	}
}