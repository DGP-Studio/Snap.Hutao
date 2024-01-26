// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Core.IO.Http.DynamicProxy;
using Snap.Hutao.Core.IO.Http.Loopback;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Algolia;
using Snap.Hutao.Web.Response;
using System.Runtime.InteropServices;
using Windows.System;

namespace Snap.Hutao.ViewModel.Feedback;

[Injection(InjectAs.Scoped)]
[ConstructorGenerated]
internal sealed partial class FeedbackViewModel : Abstraction.ViewModel
{
    private readonly HutaoInfrastructureClient hutaoInfrastructureClient;
    private readonly HutaoDocumentationClient hutaoDocumentationClient;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IClipboardProvider clipboardProvider;
    private readonly DynamicHttpProxy dynamicHttpProxy;
    private readonly LoopbackManager loopbackManager;
    private readonly IInfoBarService infoBarService;
    private readonly CultureOptions cultureOptions;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    private string? searchText;
    private List<AlgoliaHit>? searchResults;
    private IPInformation? ipInformation;

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public DynamicHttpProxy DynamicHttpProxy { get => dynamicHttpProxy; }

    public LoopbackManager LoopbackManager { get => loopbackManager; }

    public string? SearchText { get => searchText; set => SetProperty(ref searchText, value); }

    public List<AlgoliaHit>? SearchResults { get => searchResults; set => SetProperty(ref searchResults, value); }

    public IPInformation? IPInformation { get => ipInformation; private set => SetProperty(ref ipInformation, value); }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        Response<IPInformation> resp = await hutaoInfrastructureClient.GetIPInformationAsync().ConfigureAwait(false);
        IPInformation info;

        if (resp.IsOk())
        {
            info = resp.Data;
        }
        else
        {
            info = IPInformation.Default;
        }

        await taskContext.SwitchToMainThreadAsync();
        IPInformation = info;

        return true;
    }

    [Command("NavigateToUriCommand")]
    private static async Task NavigateToUri(string? uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            return;
        }

        await Launcher.LaunchUriAsync(uri.ToUri());
    }

    [Command("SearchDocumentCommand")]
    private async Task SearchDocumentAsync(string? searchText)
    {
        IsInitialized = false;
        SearchResults = null;

        if (string.IsNullOrEmpty(searchText))
        {
            IsInitialized = true;
            return;
        }

        string language = cultureOptions.GetLanguageCodeForDocumentationSearch();
        AlgoliaResponse? response = await hutaoDocumentationClient.QueryAsync(searchText, language).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        if (response is { Results: [AlgoliaResult { Hits: { Count: > 0 } hits }, ..] })
        {
            SearchResults = [.. hits.DistinctBy(hit => hit.Url)];
        }
        else
        {
            SearchResults = null;
        }

        IsInitialized = true;
    }

    [Command("CopyDeviceIdCommand")]
    private void CopyDeviceId()
    {
        try
        {
            clipboardProvider.SetText(RuntimeOptions.DeviceId);
            infoBarService.Success(SH.ViewModelSettingCopyDeviceIdSuccess);
        }
        catch (COMException ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("EnableLoopbackCommand")]
    private async Task EnableLoopbackAsync()
    {
        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(SH.ViewDialogFeedbackEnableLoopbackTitle, SH.ViewDialogFeedbackEnableLoopbackContent)
            .ConfigureAwait(false);

        if (result is ContentDialogResult.Primary)
        {
            LoopbackManager.EnableLoopback();
        }
    }
}