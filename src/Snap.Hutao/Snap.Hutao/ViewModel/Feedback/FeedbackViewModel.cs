// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.IO.Http.Loopback;
using Snap.Hutao.Core.IO.Http.Proxy;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Algolia;
using Snap.Hutao.Web.Hutao.Issue;
using Snap.Hutao.Web.Response;
using System.Runtime.InteropServices;
using Windows.System;

namespace Snap.Hutao.ViewModel.Feedback;

[Service(ServiceLifetime.Scoped)]
[ConstructorGenerated]
internal sealed partial class FeedbackViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IClipboardProvider clipboardProvider;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;

    public static HttpProxyUsingSystemProxy DynamicHttpProxy { get => HttpProxyUsingSystemProxy.Instance; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial LoopbackSupport LoopbackSupport { get; }

    public string? SearchText { get; set => SetProperty(ref field, value); }

    public List<AlgoliaHit>? SearchResults { get; set => SetProperty(ref field, value); }

    public IPInformation? IPInformation { get; private set => SetProperty(ref field, value); }

    public List<BugIssueItem>? WaitingForRelease { get; private set => SetProperty(ref field, value); }

    public List<BugIssueItem>? Untreated { get; private set => SetProperty(ref field, value); }

    public List<BugIssueItem>? HardToFix { get; private set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        IPInformation? info;
        HutaoResponse<BugIssuePayload>? bugResponse;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoInfrastructureClient hutaoInfrastructureClient = scope.ServiceProvider.GetRequiredService<HutaoInfrastructureClient>();
            Response<IPInformation> resp = await hutaoInfrastructureClient.GetIPInformationAsync(token).ConfigureAwait(false);
            ResponseValidator.TryValidate(resp, infoBarService, out info);

            bugResponse = await hutaoInfrastructureClient.GetBugIssuesAsync(token).ConfigureAwait(false);
        }

        info ??= IPInformation.Default;
        await taskContext.SwitchToMainThreadAsync();
        IPInformation = info;

        if (ResponseValidator.TryValidate(bugResponse, infoBarService, out BugIssuePayload? bugs))
        {
            // Categorize based on label rules
            List<BugIssueItem> waiting = new();
            List<BugIssueItem> untreated = new();
            List<BugIssueItem> hard = new();

            if (bugs is not null)
            {
                foreach (BugIssueItem item in bugs.Details)
                {
                    bool isHard = item.Labels.Any(l => string.Equals(l, "needs-community-help", StringComparison.OrdinalIgnoreCase) || l.Contains("无法稳定复现", StringComparison.OrdinalIgnoreCase));
                    if (isHard)
                    {
                        hard.Add(item);
                        continue;
                    }

                    // No explicit marker for waiting_for_release in details; we use stats to hint but fallback to title heuristics
                    bool seemsFixed = item.Labels.Any(l => l.Contains("fixed", StringComparison.OrdinalIgnoreCase) || l.Contains("已修复", StringComparison.OrdinalIgnoreCase))
                        || item.Title.Contains("[Fixed]", StringComparison.OrdinalIgnoreCase) || item.Title.Contains("已修复", StringComparison.OrdinalIgnoreCase);

                    if (seemsFixed)
                    {
                        waiting.Add(item);
                    }
                    else
                    {
                        untreated.Add(item);
                    }
                }
            }

            WaitingForRelease = waiting.Count > 0 ? waiting : null;
            Untreated = untreated.Count > 0 ? untreated : null;
            HardToFix = hard.Count > 0 ? hard : null;
        }

        return true;
    }

    [Command("NavigateToUriCommand")]
    private static async Task NavigateToUri(string? uri)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Navigate to uri", "FeedbackViewModel.Command", [("uri", uri ?? "<null>")]));

        if (string.IsNullOrEmpty(uri))
        {
            return;
        }

        await Launcher.LaunchUriAsync(uri.ToUri());
    }

    [Command("SearchDocumentCommand")]
    private async Task SearchDocumentAsync(string? search)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Search", "FeedbackViewModel.Command", [("text", search ?? "<null>")]));

        IsInitialized = false;
        SearchResults = null;

        if (string.IsNullOrEmpty(search))
        {
            IsInitialized = true;
            return;
        }

        string language = cultureOptions.GetLanguageCodeForDocumentationSearch();
        AlgoliaResponse? response;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoDocumentationClient hutaoDocumentationClient = scope.ServiceProvider.GetRequiredService<HutaoDocumentationClient>();
            response = await hutaoDocumentationClient.QueryAsync(search, language).ConfigureAwait(false);
        }

        await taskContext.SwitchToMainThreadAsync();
        if (response is { Results: [{ Hits: { Count: > 0 } hits }, ..] })
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
    private async Task CopyDeviceIdAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Copy device id", "FeedbackViewModel.Command"));

        try
        {
            await clipboardProvider.SetTextAsync(HutaoRuntime.DeviceId).ConfigureAwait(false);
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
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Enable Loopback", "FeedbackViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(SH.ViewDialogFeedbackEnableLoopbackTitle, SH.ViewDialogFeedbackEnableLoopbackContent)
            .ConfigureAwait(false);

        if (result is ContentDialogResult.Primary)
        {
            await taskContext.SwitchToMainThreadAsync();
            LoopbackSupport.EnableLoopback();
        }
    }

    [Command("ReportIssueCommand")]
    private static async Task ReportIssueAsync()
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/DGP-Studio/Snap.Hutao/issues/new/choose"));
    }
}