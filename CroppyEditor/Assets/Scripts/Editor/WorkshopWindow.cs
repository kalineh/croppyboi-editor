using System.IO;
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
    private string textFolder;

    public void OnGUI()
    {
        var valid = true;

        if (SteamUnity.Instance == null)
        {
            EditorGUILayout.HelpBox("Missing SteamUnity object", MessageType.None);
            if (GUILayout.Button("Create"))
                SteamUnity.Instance = (new GameObject("SteamUnity")).AddComponent<SteamUnity>();

            return;
        }

        EditorGUILayout.HelpBox("Steam", MessageType.None);

        var steamid = (int)((ulong)SteamUser.GetSteamID().m_SteamID);
        var steamname = SteamFriends.GetPersonaName();

        GUILayout.Label(string.Format("Initialized: {0}", SteamManager.Initialized.ToString()));
        GUILayout.Label(string.Format("Steam Running: {0}", SteamAPI.IsSteamRunning()));
        GUILayout.Label(string.Format("Steam ID: {0}", steamid));
        GUILayout.Label(string.Format("Steam User: {0}", steamname));
        GUILayout.Label(string.Format("Steam User: {0}", steamname));

        if (GUILayout.Button("Subscription info"))
        {
            var subscribed = SteamUGC.GetNumSubscribedItems();
            var items = new PublishedFileId_t[subscribed];
            var count = SteamUGC.GetSubscribedItems(items, subscribed);

            Debug.LogFormat("SteamUnity: found {0} subscribed items...", count);
        }

        EditorGUILayout.HelpBox("Steam Utilities", MessageType.None);

        if (GUILayout.Button("Create Workshop Item"))
            SteamUnity.Instance.CreateItem();

        if (GUILayout.Button("Scan Published Items"))
            SteamUnity.Instance.QueryPublishedItems();

        if (GUILayout.Button("Submit Item"))
            SteamUnity.Instance.SubmitItem(textFolder, textTag, textTitle, textDescription);

        EditorGUILayout.HelpBox("Steam Items", MessageType.None);

        var published = SteamUnity.Instance.GetPublishedItems();
        var working = SteamUnity.Instance.GetWorking();

        for (int i = 0; i < published.Count; ++i)
        {
            var item = published[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("{0} ({1})", item.m_rgchTitle, item.m_nPublishedFileId.m_PublishedFileId.ToString()));
            if (GUILayout.Button("Select"))
            {
                SteamUnity.Instance.SelectWorking(item.m_nPublishedFileId);

                var details = SteamUnity.Instance.GetWorkingDetails();

                textTitle = details.m_rgchTitle;
                textDescription = details.m_rgchDescription;
                textTag = details.m_rgchTags;
                textFolder = details.m_pchFileName;
            }

            GUILayout.EndHorizontal();
        }

        EditorGUILayout.HelpBox("Working Item", MessageType.None);

        GUILayout.Label(string.Format("ID: {0}", working));

        if (working != PublishedFileId_t.Invalid)
        {
            var details = SteamUnity.Instance.GetWorkingDetails();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Title");
            textTitle = GUILayout.TextField(textTitle);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Description");
            textDescription = GUILayout.TextField(textDescription);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tag");
            textTag = GUILayout.TextField(textTag);
            if (textTag != "character" && textTag != "world")
                textTag = "world";
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Folder");
            textFolder = GUILayout.TextField(textFolder);
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

                        menu.AddItem(new GUIContent(contentName), textFolder == content, o => { textFolder = Path.GetDirectoryName(content); }, null);
                    }

                    menu.ShowAsContext();
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
