using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Content;

public class ExportWorld
    : MonoBehaviour
{
    //[MenuItem("Export/Fix Sprite Missing #%K")]
    public static void MenuFixSpriteClean()
    {
        var sel = UnityEditor.Selection.activeGameObject;
        if (sel.transform.Find("Sprite") != null)
            sel = sel.transform.Find("Sprite").gameObject;
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(sel);
        sel.AddComponent<SpriteSortY_Static>();
        EditorUtility.SetDirty(sel);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Export/Validate World")]
    public static void MenuValidateWorld()
    {
        var time0 = EditorApplication.timeSinceStartup;
        Debug.LogFormat("Validate: validating world...");

        if (GameObject.FindObjectsOfType<SpawnPointProxy>().Length == 0)
            Debug.LogErrorFormat("Validate: no spawn points found");
        if (GameObject.FindObjectsOfType<FarmZoneProxy>().Length == 0)
            Debug.LogWarningFormat("Validate: no farm zones found");

        foreach (var farmZone in GameObject.FindObjectsOfType<FarmZoneProxy>())
        {
            if (farmZone.GetComponent<Collider2D>() == null)
                Debug.LogErrorFormat(farmZone.gameObject, "Validate: farm zone missing Collider2D");
            if (farmZone.claimBoardProxy == null)
                Debug.LogWarningFormat(farmZone.gameObject, "Validate: farm zone missing claim board");
            if (farmZone.shippingBoxProxy == null)
                Debug.LogWarningFormat(farmZone.gameObject, "Validate: farm zone missing shipping box");
        }

        foreach (var farmHouse in GameObject.FindObjectsOfType<FarmHouseProxy>())
        {
            if (farmHouse.farmZone == null)
                Debug.LogErrorFormat(farmHouse.gameObject, "Validate: farm house missing farm zone");
        }

        foreach (var coupZone in GameObject.FindObjectsOfType<ChickenCoupProxy>())
        {
            if (coupZone.gameObject.GetComponentInChildren<ChickenCoupZoneProxy>() == null)
                Debug.LogErrorFormat(coupZone.gameObject, "Validate: chicken coup missing coup zone child");
        }

        var time1 = EditorApplication.timeSinceStartup;
        Debug.LogFormat("Validate: complete in {0}ms", (int)((time1 - time0) * 1000.0f));
    }

    //[MenuItem("Export/Export World")]
    public static void MenuExportWorld()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "../Exports/Worlds"));

        Debug.LogFormat("Export: exporting to {0}...", path);

        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.AssetBundleStripUnityVersion, BuildTarget.StandaloneWindows);
    }
}
