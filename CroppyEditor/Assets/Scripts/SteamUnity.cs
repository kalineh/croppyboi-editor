﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

#if UNITY_STANDALONE
using Steamworks;
#endif

[ExecuteInEditMode]
public class SteamUnity
    : MonoBehaviour
{
    public static SteamUnity Instance;

    private AppId_t SteamAppId = new AppId_t(1369400);

    private CallResult<CreateItemResult_t> OnCreateItemResult;
    private CallResult<SubmitItemUpdateResult_t> OnSubmitItemUpdateResult;
    private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompleted;

    private bool busy;
    private PublishedFileId_t working;
    private List<SteamUGCDetails_t> publishedItems = new List<SteamUGCDetails_t>();
    private UGCQueryHandle_t query;

    public void OnEnable()
    {
        Instance = this;

        //SteamAPI.Init();
        //SteamAPI.RestartAppIfNecessary(SteamAppId);

        OnCreateItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResultFunc);
        OnSubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResultFunc);
        OnSteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompletedCallFunc);
    }

    public void Cleanup()
    {
        if (OnCreateItemResult != null)
            OnCreateItemResult.Dispose();
        OnCreateItemResult = null;

        if (OnSubmitItemUpdateResult != null)
            OnSubmitItemUpdateResult.Dispose();
        OnSubmitItemUpdateResult = null;
    }

    public void OnDestroy()
    {
        Cleanup();
    }

    public void OnDisable()
    {
        Cleanup();
    }

    public void CheckLegal(PublishedFileId_t publish)
    {
        var url = string.Format("steam://url/CommunityFilePage/{0}", publish);

        SteamFriends.ActivateGameOverlayToWebPage(url);
    }

    public PublishedFileId_t GetWorking()
    {
        return working;
    }

    public SteamUGCDetails_t GetWorkingDetails()
    {
        for (int i = 0; i < publishedItems.Count; ++i)
        {
            if (publishedItems[i].m_nPublishedFileId == working)
                return publishedItems[i];
        }

        return new SteamUGCDetails_t();
    }

    public void SelectWorking(PublishedFileId_t id)
    {
        if (busy)
            return;
        busy = true;

        working = id;

        busy = false;
    }

    public void CreateItem()
    {
        if (busy)
            return;
        busy = true;

        Debug.LogFormat("SteamUnity.SubmitItem: creating new item...");

        var create = SteamUGC.CreateItem(SteamAppId, EWorkshopFileType.k_EWorkshopFileTypeCommunity);

        OnCreateItemResult.Set(create);
    }

    public void OnCreateItemResultFunc(CreateItemResult_t result, bool ioFailure)
    {
        Debug.LogFormat("SteamUnity.CreateItem: result: {0}, {1}", result.m_eResult, result.m_nPublishedFileId);

        if (result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
            CheckLegal(result.m_nPublishedFileId);

        working = result.m_nPublishedFileId;

        busy = false;
    }

    public void SubmitItem(string folder, string tag, string title, string description)
    {
        if (busy)
            return;
        busy = true;

        if (working == PublishedFileId_t.Invalid)
        {
            Debug.LogFormat("SteamUnity.SubmitItemUpdate: no workshop item set...");
            return;
        }

        Debug.LogFormat("SteamUnity.SubmitItem: updating item...");

        var preview = System.IO.Path.Combine(folder, "preview.jpg");
        var tags = new List<string>() { tag, };

        var update = SteamUGC.StartItemUpdate(SteamAppId, working);

        SteamUGC.SetItemTitle(update, title);
        SteamUGC.SetItemDescription(update, description);
        SteamUGC.SetItemMetadata(update, "metadata");
        SteamUGC.SetItemVisibility(update, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
        SteamUGC.SetItemTags(update, tags);
        SteamUGC.SetItemContent(update, folder);
        SteamUGC.SetItemPreview(update, preview);

        var submit = SteamUGC.SubmitItemUpdate(update, "update");

        OnSubmitItemUpdateResult.Set(submit);
    }

    public void OnSubmitItemUpdateResultFunc(SubmitItemUpdateResult_t result, bool ioFailure)
    {
        Debug.LogFormat("SteamUnity.SubmitItemUpdate: result: {0}, {1}", result.m_eResult, result.m_nPublishedFileId);

        if (result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
            CheckLegal(result.m_nPublishedFileId);

        working = result.m_nPublishedFileId;

        busy = false;
    }

    public List<SteamUGCDetails_t> GetPublishedItems()
    {
        return this.publishedItems;
    }

    public void QueryPublishedItems()
    {
        if (busy)
            return;
        busy = true;

        var steamId = SteamUser.GetSteamID();
        var accountId = SteamUser.GetSteamID().GetAccountID();
        var page = 1;

        query = SteamUGC.CreateQueryUserUGCRequest(accountId, EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_All, EUserUGCListSortOrder.k_EUserUGCListSortOrder_LastUpdatedDesc, SteamAppId, SteamAppId, (uint)page);

        var handle = SteamUGC.SendQueryUGCRequest(query);

        OnSteamUGCQueryCompleted.Set(handle);
    }

    public void OnSteamUGCQueryCompletedCallFunc(SteamUGCQueryCompleted_t result, bool ioFailure)
    {
        Debug.LogFormat("SteamUnity.OnSteamUGCQueryCompletedCallFunc: result: {0} (total: {1})", result.m_unNumResultsReturned, result.m_unTotalMatchingResults);

        publishedItems.Clear();

        for (int i = 0; i < result.m_unNumResultsReturned; ++i)
        {
            var details = new SteamUGCDetails_t();

            SteamUGC.GetQueryUGCResult(query, (uint)i, out details);

            publishedItems.Add(details);
        }

        busy = false;
    }
}