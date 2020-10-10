using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Content;

public class ExportCharacter
    : MonoBehaviour
{
    [MenuItem("Export/Validate Character")]
    public static void MenuValidateCharacter()
    {
        var time0 = EditorApplication.timeSinceStartup;
        Debug.LogFormat("Validate: validating character...");

        // TODO: check sprites

        var time1 = EditorApplication.timeSinceStartup;
        Debug.LogFormat("Validate: complete in {0}ms", (int)((time1 - time0) * 1000.0f));
    }

    //[MenuItem("Export/Export Character")]
    public static void MenuExportCharacter()
    {
        var selectedObj = UnityEditor.Selection.activeGameObject;
        if (selectedObj == null)
        {
            Debug.LogFormat("Validate: select a character object to export");
            return;
        }

        var selected = selectedObj.GetComponent<CustomBundleCharacterData>();
        if (selected == null)
        {
            Debug.LogFormat("Validate: missing CustomBundleCharacterData component");
            return;
        }

        var character = selected;
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "../Exports/Characters"));

        Debug.LogFormat("Export: exporting to {0}...", path);

        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        var assets = new List<string>();

        assets.Add(AssetDatabase.GetAssetPath(character));

        var build = new AssetBundleBuild()
        {
            assetBundleName = "customcharacter",
            assetBundleVariant = null,
            assetNames = new string[] { AssetDatabase.GetAssetPath(selectedObj), },
            addressableNames = new string[] { "data", },
        };

        BuildPipeline.BuildAssetBundles(path, new AssetBundleBuild[] { build, }, BuildAssetBundleOptions.AssetBundleStripUnityVersion, BuildTarget.StandaloneWindows);
    }
}
