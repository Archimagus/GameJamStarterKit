using UnityEditor;
using System.Linq;
using System.IO.Compression;
using UnityEngine;

class BatchBuilder
{
	[MenuItem("Tools/Batch Build")]
	public static void BuildGame()
	{
		if (EditorUtility.DisplayDialog("Batch Build", "Batch build for\nWebGL\nWindows\nMax\nLinux", "OK", "Cancel"))
		{
			var levels = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

			Debug.Log("<Color=red>Building WebGL</color>");
			BuildPipeline.BuildPlayer(levels, $"Build/{PlayerSettings.productName}WebGL", BuildTarget.WebGL, BuildOptions.None);
			Debug.Log("<Color=red>Building Windows</color>");
			BuildPipeline.BuildPlayer(levels, $"Build/{PlayerSettings.productName}Windows/{PlayerSettings.productName}.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
			Debug.Log("<Color=red>Building Mac</color>");
			BuildPipeline.BuildPlayer(levels, $"Build/{PlayerSettings.productName}Mac.app", BuildTarget.StandaloneOSX, BuildOptions.None);
			Debug.Log("<Color=red>Building Linux</color>");
			BuildPipeline.BuildPlayer(levels, $"Build/{PlayerSettings.productName}Linux/{PlayerSettings.productName}.x64", BuildTarget.StandaloneLinux64, BuildOptions.None);
			Debug.Log("<Color=red>Build Complete</color>");
		}
	}
}