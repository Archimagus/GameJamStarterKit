using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AudioPostProcessor : AssetPostprocessor
{
	void OnPostprocessAudio(AudioClip c)
	{
		Apply();
	}
	[MenuItem("Tools/Fix AudioClips")]
	private static void Apply()
	{
		string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio/Resources" });
		var db= ScriptableObject.CreateInstance<AudioDatabase>();
		db.AudioClips.Clear();
		db.ClipTypes.Clear();
		AssetDatabase.CreateAsset(db, $"Assets/Audio/Resources/AudioDatabase.asset");

		var sb = new StringBuilder();
		sb.AppendLine("// Auto Generated File, Don't Modify");
		sb.AppendLine("public enum AudioClips");
		sb.AppendLine("{");
		int count = 0;
		foreach (string g in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(g);
			var name = Path.GetFileNameWithoutExtension(path);
			sb.AppendLine($"\t{name},");

			var directory =  Path.GetDirectoryName(path);
			directory = directory.Substring(directory.LastIndexOf('\\')+1);
			if (directory == "Resources")
				directory = "Default";
			db.AudioClips.Add((AudioClips)count, 
				new AudioClipData(AssetDatabase.LoadAssetAtPath<AudioClip>(path), 
				(SoundType)System.Enum.Parse(typeof(SoundType), directory)));
			count++;
		}

		sb.Remove(sb.Length - 3, 1);
		sb.AppendLine("}");
		var dataPath = Application.dataPath.Replace('/','\\');
		dataPath = Path.Combine(dataPath, @"Scripts\Utilities\AudioClips.cs");
		File.WriteAllText(dataPath, sb.ToString());
		AssetDatabase.SaveAssets();
		AssetDatabase.ImportAsset(@"Assets/Scripts/Utilities/AudioClips.cs");
		AssetDatabase.Refresh();
	}
}
