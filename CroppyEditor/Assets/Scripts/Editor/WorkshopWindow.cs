﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Content;
using Steamworks;

public class WorkshopWindow
    : EditorWindow
{
    [MenuItem("Export/Workshop Window")]
    public static void MenuWorkshopWindow()
    {
        EditorWindow.GetWindow(typeof(WorkshopWindow));
    }

    private void OnEnable()
    {
    }

    private void Refresh()
    {
    }

    [ExecuteInEditMode]
    public void Update()
    {
        SteamAPI.RunCallbacks();
    }

    private static AppId_t SteamAppId = new AppId_t(1369400);

    private List<PublishedFileId_t> publishedFiles = new List<PublishedFileId_t>();

    private string textTitle;
    private string textDescription;
    private string textTag;
    private string textManifest;
    private Texture2D texturePreview;

    public void OnGUI()
    {
        var valid = true;

        EditorSteamManager.EnforceInstance();
        EditorSteamManager.PrepareSteamworks();

        if (SteamUnity.Instance == null)
            SteamUnity.Instance = GameObject.FindObjectOfType<SteamUnity>();
        if (SteamUnity.Instance == null)
            SteamUnity.Instance = (new GameObject("SteamUnity")).AddComponent<SteamUnity>();
        if (SteamUnity.Instance == null)
            return;

        EditorGUILayout.HelpBox("Steam", MessageType.None);

        var steamid = (int)((ulong)SteamUser.GetSteamID().m_SteamID);
        var steamname = SteamFriends.GetPersonaName();

        GUILayout.Label(string.Format("Initialized: {0}", EditorSteamManager.Instance.Initialized.ToString()));
        GUILayout.Label(string.Format("Steam Running: {0}", SteamAPI.IsSteamRunning()));
        GUILayout.Label(string.Format("Steam ID: {0}", steamid));
        GUILayout.Label(string.Format("Steam User: {0}", steamname));

        if (GUILayout.Button("Break Busy"))
            SteamUnity.Instance.busy = false;

        EditorGUILayout.HelpBox("Steam Published Files", MessageType.None);

        if (GUILayout.Button("Scan Published Items"))
            SteamUnity.Instance.QueryPublishedItems();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Workshop Item"))
            SteamUnity.Instance.CreateItem();

        if (GUILayout.Button("Submit Item"))
            SteamUnity.Instance.SubmitItem(textTitle, textDescription, textTag, textManifest, texturePreview);

        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Steam Published Items", MessageType.None);

        var published = SteamUnity.Instance.GetPublishedItems();
        var working = SteamUnity.Instance.GetWorking();

        for (int i = 0; i < published.Count; ++i)
        {
            var item = published[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("{0} ({1})", item.m_rgchTitle, item.m_nPublishedFileId.m_PublishedFileId.ToString()), GUILayout.Width(200));

            if (GUILayout.Button("Select"))
            {
                SteamUnity.Instance.SelectWorking(item.m_nPublishedFileId);

                var details = SteamUnity.Instance.GetWorkingDetails();

                textTitle = details.m_rgchTitle;
                textDescription = details.m_rgchDescription;
                textTag = details.m_rgchTags;
                textManifest = details.m_pchFileName;
            }

            GUILayout.EndHorizontal();
        }

        EditorGUILayout.HelpBox("Steam Submission", MessageType.None);

        if (working != PublishedFileId_t.Invalid)
        {
            var workingDetails = SteamUnity.Instance.GetWorkingDetails();

            if (workingDetails.m_nPublishedFileId != PublishedFileId_t.Invalid)
            {
                GUILayout.Label(string.Format("{0} ({1})", workingDetails.m_rgchTitle, workingDetails.m_nPublishedFileId), GUILayout.Width(200));

                GUILayout.BeginHorizontal();
                GUILayout.Label("Title", GUILayout.Width(200));
                textTitle = GUILayout.TextField(textTitle);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Description", GUILayout.Width(200));
                textDescription = GUILayout.TextField(textDescription);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Tag", GUILayout.Width(200));
                textTag = GUILayout.TextField(textTag);
                if (textTag != "character" && textTag != "world")
                    textTag = "world";
                GUILayout.EndHorizontal();

                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Folder", GUILayout.Width(200));
                textManifest = GUILayout.TextField(textManifest);
                var menu = new GenericMenu();
                if (GUILayout.Button("Select Bundle"))
                {
                    var exportsFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "../Exports"));
                    var subFolder = textTag == "world" ? "Worlds" : "Characters";
                    var contentFolder = Path.Combine(exportsFolder, subFolder);

                    if (Directory.Exists(contentFolder))
                    {
                        var contentFiles = new List<string>(Directory.GetFiles(contentFolder, "*.manifest"));

                        foreach (var content in contentFiles)
                        {
                            if (content == "Characters.manifest" || content == "Worlds.manifest")
                                continue;

                            var contentName = Path.GetFileNameWithoutExtension(content);

                            menu.AddItem(new GUIContent(contentName), textManifest == content, o => { textManifest = content; }, null);
                        }

                        menu.ShowAsContext();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Preview Texture", GUILayout.Width(200));
                texturePreview = EditorGUILayout.ObjectField(texturePreview, typeof(Texture2D), true) as Texture2D;
                GUILayout.EndHorizontal();
            }
        }
    }
}
