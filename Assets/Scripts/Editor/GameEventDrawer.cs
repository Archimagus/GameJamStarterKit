using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameEvent), true)]
public class GameEventDrawer: PropertyDrawer
{
	static GUIStyle _buttonStyle;
	static GameEventDrawer()
	{
		_buttonStyle = new GUIStyle();
		_buttonStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/ol plus.png") as Texture2D;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		float w = _buttonStyle.normal.background.width;
		float h = _buttonStyle.normal.background.height;
		float ratio = w / h;
		var buttonRect = new Rect(position.x, position.y+1, (position.height-2)*ratio, position.height-2);
		var valueRect = new Rect(position.x + w+2, position.y, position.width - (w+2), position.height);
		if (GUI.Button(buttonRect, GUIContent.none, _buttonStyle))
		{
			PopupWindow.Show(buttonRect, new PopupGameEventCreatorWindow() { _variableProperty = property });
		}

		EditorGUI.PropertyField(valueRect, property, GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
	public class PopupGameEventCreatorWindow : PopupWindowContent
	{
		string _name;
		public SerializedProperty _variableProperty;
		private bool _focusSet;

		public override Vector2 GetWindowSize()
		{
			return new Vector2(300, EditorGUIUtility.singleLineHeight * 1.5f);
		}

		public override void OnGUI(Rect rect)
		{
			EditorGUILayout.BeginHorizontal();
			var label = new GUIContent("Name");
			var labelSize = GUI.skin.label.CalcSize(label);
			EditorGUIUtility.labelWidth = labelSize.x + 10;
			EditorGUILayout.PrefixLabel("Name");
			GUI.SetNextControlName("MyTextField");
			_name = EditorGUILayout.TextField(_name);
			if (GUILayout.Button("OK", GUILayout.ExpandWidth(false)))
			{
				createAsset();
			}
			if (Event.current.Equals(Event.KeyboardEvent("return")))
			{
				createAsset();
			}
			EditorGUILayout.EndHorizontal();
			if (!_focusSet)
			{
				EditorGUI.FocusTextInControl("MyTextField");
				_focusSet = true;
			}
		}

		private void createAsset()
		{
			bool successs = false;
			var tr = new Regex(@"PPtr<\$(\w+)>");
			var m = tr.Match(_variableProperty.type);
			if (m.Success)
			{
				var typeName = m.Groups[1].Value;
				var asset = ScriptableObject.CreateInstance(typeName);
				if (asset != null)
				{
					AssetDatabase.CreateAsset(asset, $"Assets/Prefabs/Events/Resources/{_name}.asset");
					AssetDatabase.SaveAssets();

					_variableProperty.objectReferenceValue = asset;
					_variableProperty.serializedObject.ApplyModifiedProperties();

					EditorUtility.FocusProjectWindow();
					EditorGUIUtility.PingObject(asset);
					successs = true;
				}
			}

			if (!successs)
			{
				Debug.LogWarning($"Unable to create scriptable asset type {_variableProperty.type}");
			}
			editorWindow.Close();
		}
	}
}