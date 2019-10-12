using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AudioPostProcessor : AssetPostprocessor
{
	static readonly string audioDatabasePath = $"Assets/Audio/Resources/AudioDatabase.asset";
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		var db = CreateOrGetAudioDatabase();
		try
		{
			var newClips = new List<AudioClip>();
			foreach (var path in importedAssets)
			{
				var asset = AssetDatabase.GetMainAssetTypeAtPath(path);
				if (asset?.IsAssignableFrom(typeof(AudioClip)) ?? false)
				{
					var name = Path.GetFileNameWithoutExtension(path);
					// if AudioClips doesn't already contain an entry for the new clip then add it to the list (if we are getting from source control, it might have been added already) 
					if (!Enum.TryParse(name, out AudioClips clip))
					{
						var c = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
						newClips.Add(c);
					}
				}
			}

			var deletedClips = new HashSet<AudioClips>();
			foreach (var path in deletedAssets)
			{
				string name = Path.GetFileNameWithoutExtension(path);
				// if AudioClips contains an entry for the deleted clip add it to the list (if we are getting from source control, it might have been removed already) 
				if (Enum.TryParse(name, out AudioClips clip))
				{
					db.Remove((ulong)clip);
					deletedClips.Add(clip);
				}
			}

			foreach (var path in movedAssets)
			{
				if (AssetDatabase.GetMainAssetTypeAtPath(path).IsAssignableFrom(typeof(AudioClip)))
				{
					var name = Path.GetFileNameWithoutExtension(path);
					if (Enum.TryParse(name, out AudioClips clip))
					{
						var directory = Path.GetDirectoryName(path);
						directory = directory.Substring(directory.LastIndexOf('\\') + 1);
						if (directory == "Resources")
							directory = "Default";

						if (Enum.TryParse(directory, out SoundType t))
						{
							var c = db[clip];
							if (c != null)
								c.SoundType = t;
							else
								newClips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(path));
						}
						else
						{
							// removed from the audio resources folder
							db.Remove((ulong)clip);
							deletedClips.Add(clip);
						}
					}
					else
						newClips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(path));
				}
			}

			if (deletedClips.Any() || newClips.Any())
			{
				var sb = new StringBuilder();
				sb.AppendLine("// Auto Generated File, Don't Modify");
				sb.AppendLine("public enum AudioClips:ulong");
				sb.AppendLine("{");
				foreach (AudioClips c in Enum.GetValues(typeof(AudioClips)))
				{
					if (!deletedClips.Contains(c))
					{
						sb.AppendLine($"\t{c} = {(long)c},");
					}
				}
				foreach (var clip in newClips)
				{
					var path = AssetDatabase.GetAssetPath(clip);
					var guid = AssetDatabase.AssetPathToGUID(path);
					var name = Path.GetFileNameWithoutExtension(path);
					var directory = Path.GetDirectoryName(path);
					directory = directory.Substring(directory.LastIndexOf('\\') + 1);
					if (directory == "Resources")
						directory = "Default";

					var id = BitConverter.ToUInt64(Encoding.ASCII.GetBytes(guid), 0);
					// make sure the enum doesn't already contain this file. 
					// An old instance of AudioClips may be loaded because of an error in another scrips caused by removing an audio clip it was referencing
					if (!Enum.IsDefined(typeof(AudioClips), id))
						sb.AppendLine($"\t{name} = {id},");

					db.Add(id, clip, (SoundType)Enum.Parse(typeof(SoundType), directory));
				}
				sb.Remove(sb.Length - 3, 1);
				sb.AppendLine("}");
				var dataPath = Application.dataPath.Replace('/', '\\');
				dataPath = Path.Combine(dataPath, @"Scripts\Utilities\AudioClips.cs");
				File.WriteAllText(dataPath, sb.ToString());
				AssetDatabase.SaveAssets();
				AssetDatabase.ImportAsset(@"Assets/Scripts/Utilities/AudioClips.cs");
				AssetDatabase.Refresh();
				EditorUtility.SetDirty(db);
			}
		}
		catch (Exception exc)
		{
			EditorGUIUtility.PingObject(db);
			Debug.LogError(exc.Message);
		}

	}


	[MenuItem("Tools/Fix AudioClips")]
	private static void FixAudioClips()
	{
		string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio/Resources" });
		var db = CreateOrGetAudioDatabase();
		db.Clear();

		var sb = new StringBuilder();
		sb.AppendLine("// Auto Generated File, Don't Modify");
		sb.AppendLine("public enum AudioClips:ulong");
		sb.AppendLine("{");
		foreach (string g in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(g);
			var name = Path.GetFileNameWithoutExtension(path);
			var directory = Path.GetDirectoryName(path);
			var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
			var bytes = Encoding.ASCII.GetBytes(g);
			var id = BitConverter.ToUInt64(bytes, 0);
			sb.AppendLine($"\t{name} = {id},");
			directory = directory.Substring(directory.LastIndexOf('\\') + 1);
			if (directory == "Resources")
				directory = "Default";
			db.Add(id, clip, (SoundType)Enum.Parse(typeof(SoundType), directory));
		}

		sb.Remove(sb.Length - 3, 1);
		sb.AppendLine("}");
		var dataPath = Application.dataPath.Replace('/', '\\');
		dataPath = Path.Combine(dataPath, @"Scripts\Utilities\AudioClips.cs");
		File.WriteAllText(dataPath, sb.ToString());
		AssetDatabase.SaveAssets();
		AssetDatabase.ImportAsset(@"Assets/Scripts/Utilities/AudioClips.cs");
		EditorUtility.SetDirty(db);
		AssetDatabase.Refresh();
	}


	private static AudioDatabase CreateOrGetAudioDatabase()
	{
		var db = AssetDatabase.LoadAssetAtPath<AudioDatabase>(audioDatabasePath);
		if (db == null)
		{
			db = ScriptableObject.CreateInstance<AudioDatabase>();
			AssetDatabase.CreateAsset(db, audioDatabasePath);
		}
		return db;
	}
}
