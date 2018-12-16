using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using UnityEditor.SceneManagement;
using System.Linq;

namespace Archimagus
{
	public class ScenesWindow : EditorWindow
	{
		int sceneIndex;
		string currentScene;
		List<string> scenes = new List<string>();
		string[] sceneNames = new string[0];
		internal Dictionary<string, bool> validDirectories = new Dictionary<string, bool>();

		[MenuItem("Window/Scenes Window")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			ScenesWindow window = (ScenesWindow)GetWindow(typeof(ScenesWindow), false, "Scenes");
			window.minSize = new Vector2(100, 25);
			window.RefillScenes();
		}
		void OnEnable()
		{
			// Load the window settings;
			validDirectories.Clear();
			string settings = EditorUserSettings.GetConfigValue("ScenesWindowSettings");
			if (!string.IsNullOrEmpty(settings))
			{
				StringReader sr = new StringReader(settings);
				while (sr.Peek() > 0)
				{
					string line = sr.ReadLine();
					string[] split = line.Split(';');
					validDirectories.Add(split[0], bool.Parse(split[1]));
				}
			}
			wantsMouseEnterLeaveWindow = true;
			EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
		}
		

		private void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
		{
			currentScene = scene.path;
			sceneIndex = scenes.IndexOf(currentScene);
			Repaint();
		}
		
		void OnGUI()
		{
			if(Event.current.type == EventType.MouseEnterWindow || !scenes.Any())
			{
				RefillScenes();
			}
			minSize = new Vector2(100, 23);
			var oldIndex = sceneIndex;
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("<<"))
			{
				sceneIndex--;
				if (sceneIndex < 0)
					sceneIndex = scenes.Count - 1;
			}
			sceneIndex = EditorGUILayout.Popup(sceneIndex, sceneNames.ToArray());
			if (GUILayout.Button(">>"))
			{
				sceneIndex++;
				if (sceneIndex >= scenes.Count)
					sceneIndex = 0;
			}
			if (GUILayout.Button(EditorGUIUtility.IconContent("_Help")))
			{
				ScenesWindowSettings w = (ScenesWindowSettings)EditorWindow.GetWindow(typeof(ScenesWindowSettings), true, "Settings");
				w.Init(this);
			}
			if (sceneIndex != oldIndex)
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.OpenScene(scenes[sceneIndex]);
				
				var sceneView = (SceneView.sceneViews[0] as SceneView);
				sceneView.FrameSelected();
				Selection.activeObject = null;
			}
		}

		private void RefillScenes()
		{
			scenes.Clear();
			var names = new List<string>();
			var paths = AssetDatabase.GetAllAssetPaths();
			foreach (var p in paths)
			{
				if (Path.GetExtension(p) == ".unity")
				{
					var d = Path.GetDirectoryName(p);
					if (!validDirectories.ContainsKey(d))
					{
						validDirectories.Add(d, true);
					}
					if (validDirectories[d] == false)
						continue;
					scenes.Add(p);
					names.Add(Path.GetFileNameWithoutExtension(p));
				}
			}
			sceneNames = names.ToArray();
			currentScene = EditorSceneManager.GetActiveScene().path;
			sceneIndex = scenes.IndexOf(currentScene);
			Repaint();
		}
	}

	class ScenesWindowSettings : EditorWindow
	{
		ScenesWindow _scenesWindow;
		Vector2 _scrollPos = new Vector2();
		internal void Init(ScenesWindow scenesWindow)
		{
			_scenesWindow = scenesWindow;
		}

		void OnGUI()
		{
			if (_scenesWindow != null)
			{
				EditorGUILayout.LabelField("Valid Scenes Directories");
				_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
				var directories = _scenesWindow.validDirectories;
				List<string> keys = new List<string>();
				List<bool> values = new List<bool>();
				foreach (var d in directories)
				{
					keys.Add(d.Key);
					values.Add(d.Value);
				}
				bool dirty = false;
				for (int i = 0; i < keys.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					directories[keys[i]] = EditorGUILayout.Toggle(directories[keys[i]], GUILayout.MaxWidth(15));
					if (directories[keys[i]] != values[i])
						dirty = true;
					EditorGUILayout.LabelField(keys[i]);
					EditorGUILayout.EndHorizontal();

				}
				if (dirty)
				{
					StringBuilder sb = new StringBuilder();
					foreach (var d in directories)
					{
						sb.Append(d.Key);
						sb.Append(";");
						sb.AppendLine(d.Value.ToString());
					}
					EditorUserSettings.SetConfigValue("ScenesWindowSettings", sb.ToString());
				}
				EditorGUILayout.EndScrollView();
			}
		}
	}
}