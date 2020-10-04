using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Content;

public class ExportWindow
    : EditorWindow
{
    [MenuItem("Export/Export Window")]
    public static void MenuExportWindow()
    {
        EditorWindow.GetWindow(typeof(ExportWindow));
    }

    private List<string> assetWorldPaths = new List<string>();
    private List<string> assetCharacterPaths = new List<string>();

    private void Refresh()
    {
        assetWorldPaths = new List<string>();
        assetCharacterPaths = new List<string>();

        var assetScenes = AssetDatabase.FindAssets("t:Scene");

        for (int i = 0; i < assetScenes.Length; ++i)
        {
            var assetGuid = assetScenes[i];
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            if (assetPath.StartsWith("Assets/Bundles/Worlds"))
                assetWorldPaths.Add(assetPath);
        }

        var assetCharacters = AssetDatabase.FindAssets("t:GameObject");

        for (int i = 0; i < assetCharacters.Length; ++i)
        {
            var assetGuid = assetCharacters[i];
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            if (assetPath.StartsWith("Assets/Bundles/Characters"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (asset.GetComponent<CustomBundleCharacterData>() != null)
                    assetCharacterPaths.Add(assetPath);
            }
        }
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Refresh"))
            Refresh();

        EditorGUILayout.HelpBox("Worlds", MessageType.None);

        for (int i = 0; i < assetWorldPaths.Count; ++i)
        {
            var assetWorldPath = assetWorldPaths[i];
            var assetWorldPathBundle = AssetDatabase.GetImplicitAssetBundleName(assetWorldPath);

            if (string.IsNullOrEmpty(assetWorldPathBundle))
                assetWorldPathBundle = "MISSING";

            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("{0}", i));
            GUILayout.Label(string.Format("{0}", assetWorldPath));
            GUILayout.Label(string.Format("{0}", assetWorldPathBundle));
            if (GUILayout.Button("Open"))
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetWorldPath);
            if (GUILayout.Button("Export"))
                ExportWorld(assetWorldPath);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.HelpBox("Characters", MessageType.None);

        for (int i = 0; i < assetCharacterPaths.Count; ++i)
        {
            var assetCharacterPath = assetCharacterPaths[i];
            var assetCharacterPathBundle = AssetDatabase.GetImplicitAssetBundleName(assetCharacterPath);

            if (string.IsNullOrEmpty(assetCharacterPathBundle))
                assetCharacterPathBundle = "MISSING";

            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("{0}", i));
            GUILayout.Label(string.Format("{0}", assetCharacterPath));
            GUILayout.Label(string.Format("{0}", assetCharacterPathBundle));
            if (GUILayout.Button("Open"))
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetCharacterPath);
            if (GUILayout.Button("Export"))
                ExportCharacter(assetCharacterPath);
            GUILayout.EndHorizontal();
        }
    }

    public static void ExportWorld(string assetPath)
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "../Exports/Worlds"));

        Debug.LogFormat("Export: exporting to {0}...", path);

        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        var bundleName = AssetDatabase.GetImplicitAssetBundleName(assetPath);

        var build = new AssetBundleBuild()
        {
            assetBundleName = bundleName,
            assetBundleVariant = null,
            assetNames = new string[] { assetPath, },
            addressableNames = new string[] { "data", },
        };

        BuildPipeline.BuildAssetBundles(path, new AssetBundleBuild[] { build, }, BuildAssetBundleOptions.AssetBundleStripUnityVersion, BuildTarget.StandaloneWindows);
    }

    public static void ExportCharacter(string assetPath)
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "../Exports/Characters"));

        Debug.LogFormat("Export: exporting to {0}...", path);

        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        var bundleName = AssetDatabase.GetImplicitAssetBundleName(assetPath);

        var build = new AssetBundleBuild()
        {
            assetBundleName = bundleName,
            assetBundleVariant = null,
            assetNames = new string[] { assetPath, },
            addressableNames = new string[] { "data", },
        };

        BuildPipeline.BuildAssetBundles(path, new AssetBundleBuild[] { build, }, BuildAssetBundleOptions.AssetBundleStripUnityVersion, BuildTarget.StandaloneWindows);
    }
}
