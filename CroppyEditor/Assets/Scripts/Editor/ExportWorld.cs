using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Content;

public class ExportWorld
    : MonoBehaviour
{
    [MenuItem("Export/Export World")]
    public static void MenuExportWorld()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "../bundles"));

        Debug.LogFormat("Export: exporting to {0}...", path);

        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        // go through each proxy, store json representation
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.AssetBundleStripUnityVersion, BuildTarget.StandaloneWindows);
    }
}
