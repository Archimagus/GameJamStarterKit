using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VarReference), true)]
public class TReferenceDrawer : PropertyDrawer
{
	static GUIStyle _buttonStyle;
	static TReferenceDrawer()
	{
		_buttonStyle = new GUIStyle();
		_buttonStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/pane options.png") as Texture2D;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		var buttonRect = new Rect(position.x, position.y + 3, 18, 11);
		var popupRect = buttonRect;
		var valueRect = new Rect(position.x + 22, position.y, position.width - 22, position.height);
		if (EditorGUI.DropdownButton(buttonRect, GUIContent.none, FocusType.Passive, _buttonStyle))
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Use Constant"), property.FindPropertyRelative("UseConstant").boolValue, () =>
			{
				property.FindPropertyRelative("UseConstant").boolValue = true;
				property.serializedObject.ApplyModifiedProperties();
			});
			menu.AddItem(new GUIContent("Use Variable"), !property.FindPropertyRelative("UseConstant").boolValue, () =>
			{
				property.FindPropertyRelative("UseConstant").boolValue = false;
				property.serializedObject.ApplyModifiedProperties();
			});
			menu.AddItem(new GUIContent("New Variable"), false, () =>
			{
				property.FindPropertyRelative("UseConstant").boolValue = false;
				property.serializedObject.ApplyModifiedProperties();
				PopupWindow.Show(popupRect, new PopupTReferenceCreatorWindow() { _variableProperty = property.FindPropertyRelative("Variable") });
			});
			menu.DropDown(buttonRect);
		}

		if (property.FindPropertyRelative("UseConstant").boolValue)
			EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("ConstantValue"), GUIContent.none);
		else
			EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("Variable"), GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
	public class PopupTReferenceCreatorWindow : PopupWindowContent
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
					AssetDatabase.CreateAsset(asset, $"Assets/Prefabs/Variables/Resources/{_name}.asset");
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