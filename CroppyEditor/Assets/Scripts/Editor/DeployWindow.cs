using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Content;

public class DeployWindow
    : EditorWindow
{
    [MenuItem("Export/Deploy Window")]
    public static void MenuDeployWindow()
    {
        EditorWindow.GetWindow(typeof(DeployWindow));
    }

    [MenuItem("Custom/Forget EditorPrefs")]
    public static void MenuForgetEditorPrefs()
    {
        EditorPrefs.DeleteKey("DeployPathSrc");
        EditorPrefs.DeleteKey("DeployPathDst");
    }

    private string pathSrc;
    private string pathDst;

    private List<string> srcCharacters;
    private List<string> srcWorlds;
    private List<string> dstCharacters;
    private List<string> dstWorlds;

    private void OnEnable()
    {
        pathSrc = EditorPrefs.GetString("DeployPathSrc", Path.GetFullPath(Path.Combine(Application.dataPath, "../Exports")));
        pathDst = EditorPrefs.GetString("DeployPathDst", null);
    }

    private void StripUnwantedManifests()
    {
        for (int i = 0; i < srcCharacters.Count; ++i)
        {
            if (srcCharacters[i].EndsWith("Characters.manifest"))
                srcCharacters.RemoveAt(i--);
        }

        for (int i = 0; i < dstCharacters.Count; ++i)
        {
            if (dstCharacters[i].EndsWith("Characters.manifest"))
                dstCharacters.RemoveAt(i--);
        }

        for (int i = 0; i < srcWorlds.Count; ++i)
        {
            if (srcWorlds[i].EndsWith("Worlds.manifest"))
                srcWorlds.RemoveAt(i--);
        }

        for (int i = 0; i < dstWorlds.Count; ++i)
        {
            if (dstWorlds[i].EndsWith("Worlds.manifest"))
                dstWorlds.RemoveAt(i--);
        }
    }

    private void Refresh()
    {
        var pathSrcCharacters = Path.Combine(pathSrc, "Characters");
        var pathSrcWorlds = Path.Combine(pathSrc, "Worlds");

        var pathDstCharacters = Path.Combine(pathDst, "Characters");
        var pathDstWorlds = Path.Combine(pathDst, "Worlds");

        if (Directory.Exists(pathSrcCharacters))
            srcCharacters = new List<string>(Directory.GetFiles(pathSrcCharacters, "*.manifest"));
        else
            srcCharacters = new List<string>();

        if (Directory.Exists(pathSrcWorlds))
            srcWorlds = new List<string>(Directory.GetFiles(pathSrcWorlds, "*.manifest"));
        else
            srcWorlds = new List<string>();

        if (Directory.Exists(pathDstCharacters))
            dstCharacters = new List<string>(Directory.GetFiles(pathDstCharacters, "*.manifest"));
        else
            dstCharacters = new List<string>();

        if (Directory.Exists(pathDstWorlds))
            dstWorlds = new List<string>(Directory.GetFiles(pathDstWorlds, "*.manifest"));
        else
            dstWorlds = new List<string>();

        StripUnwantedManifests();

        EditorPrefs.SetString("DeployPathSrc", pathSrc);
        EditorPrefs.SetString("DeployPathDst", pathDst);
    }

    private void Deploy()
    {
        var srcCharactersFolder = Path.Combine(pathSrc, "Characters");
        var srcWorldsFolder = Path.Combine(pathSrc, "Worlds");
        var dstCharactersFolder = Path.Combine(pathDst, "Characters");
        var dstWorldsFolder = Path.Combine(pathDst, "Worlds");

        if (Directory.Exists(dstCharactersFolder) == false)
            Directory.CreateDirectory(dstCharactersFolder);
        if (Directory.Exists(dstWorldsFolder) == false)
            Directory.CreateDirectory(dstWorldsFolder);

        foreach (var manifest in srcCharacters)
        {
            File.Copy(manifest, Path.Combine(dstCharactersFolder, Path.GetFileName(manifest)), true);
            File.Copy(Path.Combine(srcCharactersFolder, Path.GetFileNameWithoutExtension(manifest)), Path.Combine(dstCharactersFolder, Path.GetFileNameWithoutExtension(manifest)), true);

            Debug.LogFormat("Deployed Character '{0}'...", manifest);
        }

        foreach (var manifest in srcWorlds)
        {
            File.Copy(manifest, Path.Combine(dstWorldsFolder, Path.GetFileName(manifest)), true);
            File.Copy(Path.Combine(srcWorldsFolder, Path.GetFileNameWithoutExtension(manifest)), Path.Combine(dstWorldsFolder, Path.GetFileNameWithoutExtension(manifest)), true);

            Debug.LogFormat("Deployed World '{0}'...", manifest);
        }

        Refresh();
    }

    public void OnGUI()
    {
        var valid = true;

        EditorGUILayout.HelpBox("Folders", MessageType.None);

        if (string.IsNullOrEmpty(pathDst))
        {
            EditorGUILayout.HelpBox("Export Folder not configured", MessageType.Warning);
            valid = false;
        }
        else if (Directory.Exists(pathDst) == false)
        {
            EditorGUILayout.HelpBox("Export Folder invalid path", MessageType.Warning);
            valid = false;
        }
        else if (pathDst.EndsWith("StreamingAssets") == false)
        {
            EditorGUILayout.HelpBox("Export Folder does not look like StreamingAssets folder", MessageType.Warning);
            // still valid
        }

        if (GUILayout.Button("Select Export Folder"))
        {
            var result = EditorUtility.OpenFolderPanel("Select Export Folder", "", "");
            if (result != null)
            {
                pathDst = result;
                Refresh();
            }
        }

        if (valid == false)
            return;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(pathSrc))
            Application.OpenURL(pathSrc);
        if (GUILayout.Button(pathDst))
            Application.OpenURL(pathDst);
        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Actions", MessageType.None);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh"))
            Refresh();
        if (GUILayout.Button("Deploy"))
            Deploy();
        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Source Files", MessageType.None, true);

        foreach (var w in srcWorlds)
            GUILayout.Label(w);
        foreach (var c in srcCharacters)
            GUILayout.Label(c);

        EditorGUILayout.HelpBox("Destination Files", MessageType.None, true);

        foreach (var w in dstWorlds)
            GUILayout.Label(w);
        foreach (var c in dstCharacters)
            GUILayout.Label(c);
    }
}
