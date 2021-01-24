using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TVariable), true)]
[CanEditMultipleObjects]
public class TVariableEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var clampProp = serializedObject.FindProperty("_clamp");
		if (clampProp != null)
		{
			EditorGUILayout.PropertyField(clampProp);
			bool clamp = clampProp.boolValue;
			if(clamp)
			{
				var minprop = serializedObject.FindProperty("_min");
				var maxprop = serializedObject.FindProperty("_max");
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Min/Max");
				EditorGUILayout.PropertyField(minprop, GUIContent.none);
				EditorGUILayout.PropertyField(maxprop, GUIContent.none);
				EditorGUILayout.EndHorizontal();
			}
		}

		var v = target as TVariable;
		var pi = v.GetType().GetProperty("CurrentValue", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
		if(pi != null)
		{
			var value = pi.GetValue(v);
			EditorGUILayout.LabelField(new GUIContent("Current Value"), new GUIContent(System.Convert.ToString(value)));
		}
		serializedObject.ApplyModifiedProperties();
	}
}