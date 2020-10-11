// The SteamManager is designed to work with Steamworks.NET
// This file is released into the public domain.
// Where that dedication is not recognized you are granted a perpetual,
// irrevocable license to copy and modify this file as you see fit.
//
// Version: 1.0.9

// CUSTOM: EditorSteamManager: designed to run in editor mode

#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif

using UnityEngine;
#if !DISABLESTEAMWORKS
using System.Collections;
using Steamworks;
#endif

//
// The SteamManager provides a base implementation of Steamworks.NET on which you can build upon.
// It handles the basics of starting up and shutting down the SteamAPI for use.
//
[DisallowMultipleComponent]
public class EditorSteamManager
    : MonoBehaviour
{
#if !DISABLESTEAMWORKS
    public static EditorSteamManager Instance;

    public static void EnforceInstance()
    {
        if (Instance != null)
            return;

        var existing = GameObject.FindObjectOfType<EditorSteamManager>();
        if (existing != null)
            Instance = existing;

        if (Instance == null)
            Instance = new GameObject("EditorSteamManager").AddComponent<EditorSteamManager>();

        if (!Packsize.Test())
            Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
        if (!DllCheck.Test())
            Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");

        if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
            Debug.LogError("[Steamworks.NET] Need to restart with steam running.");
	}

    public static void PrepareSteamworks()
    {
        var success = SteamAPI.Init();
        if (success == false)
            Debug.LogError("EditorSteamManager: steam init failed");

        Instance.Initialized = success;

		if (Instance.m_SteamAPIWarningMessageHook == null)
        {
			// Set up our callback to receive warning messages from Steam.
			// You must launch with "-debug_steamapi" in the launch args to receive warnings.
			Instance.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(Instance.m_SteamAPIWarningMessageHook);
		}
    }

    [System.NonSerialized]
    public bool Initialized;

	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
	protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText) {
		Debug.LogWarning(pchDebugText);
	}

	// OnApplicationQuit gets called too early to shutdown the SteamAPI.
	// Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
	// Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
	public static void ShutdownSteamworks()
    {
		SteamAPI.Shutdown();
        Instance.Initialized = false;
	}

    public void Callbacks()
    {
        SteamAPI.RunCallbacks();
    }
#endif // !DISABLESTEAMWORKS
}
