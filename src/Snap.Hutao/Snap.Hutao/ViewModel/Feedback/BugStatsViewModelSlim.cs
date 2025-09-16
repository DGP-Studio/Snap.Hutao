// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Issue;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.ViewModel.Feedback;

[Service(ServiceLifetime.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class BugStatsViewModelSlim : Abstraction.ViewModelSlim<FeedbackPage>
{
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    [ObservableProperty]
    public partial int WaitingForReleaseCount { get; set; }

    [ObservableProperty]
    public partial int UntreatedCount { get; set; }

    [ObservableProperty]
    public partial int HardToFixCount { get; set; }

    protected override async Task LoadAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoInfrastructureClient hutaoInfrastructureClient = scope.ServiceProvider.GetRequiredService<HutaoInfrastructureClient>();

            try
            {
                HutaoResponse<BugIssuePayload>? resp = await hutaoInfrastructureClient.GetBugIssuesAsync().ConfigureAwait(false);

                if (ResponseValidator.TryValidateWithoutUINotification(resp, out BugIssuePayload? payload) && payload is not null)
                {
                    await taskContext.SwitchToMainThreadAsync();
                    WaitingForReleaseCount = payload.Stat?.WaitingForRelease ?? 0;
                    UntreatedCount = payload.Stat?.Untreated ?? 0;
                    HardToFixCount = payload.Stat?.HardToFix ?? 0;
                }
            }
            catch (Exception ex)
            {
                infoBarService.Error(ex);
            }
            finally
            {
                // Always end loading state even if we failed to get data
                IsInitialized = true;
            }
        }
    }
}
