// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Guide;

/// <summary>
/// 指引视图模型
/// </summary>
[SuppressMessage("", "SA1124")]
[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class GuideViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly StaticResourceOptions staticResourceOptions;
    private readonly CultureOptions cultureOptions;
    private readonly RuntimeOptions runtimeOptions;
    private readonly AppOptions appOptions;

    private string nextOrCompleteButtonText = SH.ViewModelGuideActionNext;
    private bool isNextOrCompleteButtonEnabled = true;
    private NameValue<CultureInfo>? selectedCulture;
    private NameValue<Region>? selectedRegion;
    private bool isTermOfServiceAgreed;
    private bool isPrivacyPolicyAgreed;
    private bool isIssueReportAgreed;
    private bool isOpenSourceLicenseAgreed;
    private ObservableCollection<DownloadSummary>? downloadSummaries;

    public uint State
    {
        get
        {
            GuideState state = UnsafeLocalSetting.Get(SettingKeys.Major1Minor10Revision0GuideState, GuideState.Language);

            if (state is GuideState.Document)
            {
                IsTermOfServiceAgreed = false;
                IsPrivacyPolicyAgreed = false;
                IsIssueReportAgreed = false;
                IsOpenSourceLicenseAgreed = false;
                (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionNext, false);
            }
            else if (state is GuideState.StaticResourceBegin)
            {
                (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionStaticResourceBegin, false);
                _ = DownloadStaticResourceAsync();
            }
            else if (state is GuideState.Completed)
            {
                (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionComplete, true);
            }
            else
            {
                (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionNext, true);
            }

            return (uint)state;
        }

        set
        {
            LocalSetting.Set(SettingKeys.Major1Minor10Revision0GuideState, value);
            OnPropertyChanged();
        }
    }

    public string NextOrCompleteButtonText { get => nextOrCompleteButtonText; set => SetProperty(ref nextOrCompleteButtonText, value); }

    public bool IsNextOrCompleteButtonEnabled { get => isNextOrCompleteButtonEnabled; set => SetProperty(ref isNextOrCompleteButtonEnabled, value); }

    public CultureOptions CultureOptions { get => cultureOptions; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public AppOptions AppOptions { get => appOptions; }

    public StaticResourceOptions StaticResourceOptions { get => staticResourceOptions; }

    public NameValue<CultureInfo>? SelectedCulture
    {
        get => selectedCulture ??= CultureOptions.GetCurrentCultureForSelectionOrDefault();
        set
        {
            if (SetProperty(ref selectedCulture, value) && value is not null)
            {
                CultureOptions.CurrentCulture = value.Value;
                ++State;
                AppInstance.Restart(string.Empty);
            }
        }
    }

    public NameValue<Region>? SelectedRegion
    {
        get => selectedRegion ??= AppOptions.GetCurrentRegionForSelectionOrDefault();
        set
        {
            if (SetProperty(ref selectedRegion, value) && value is not null)
            {
                AppOptions.Region = value.Value;
            }
        }
    }

    #region Agreement
    public bool IsTermOfServiceAgreed
    {
        get => isTermOfServiceAgreed; set
        {
            if (SetProperty(ref isTermOfServiceAgreed, value))
            {
                OnAgreementStateChanged();
            }
        }
    }

    public bool IsPrivacyPolicyAgreed
    {
        get => isPrivacyPolicyAgreed; set
        {
            if (SetProperty(ref isPrivacyPolicyAgreed, value))
            {
                OnAgreementStateChanged();
            }
        }
    }

    public bool IsIssueReportAgreed
    {
        get => isIssueReportAgreed; set
        {
            if (SetProperty(ref isIssueReportAgreed, value))
            {
                OnAgreementStateChanged();
            }
        }
    }

    public bool IsOpenSourceLicenseAgreed
    {
        get => isOpenSourceLicenseAgreed; set
        {
            if (SetProperty(ref isOpenSourceLicenseAgreed, value))
            {
                OnAgreementStateChanged();
            }
        }
    }
    #endregion

    public ObservableCollection<DownloadSummary>? DownloadSummaries
    {
        get => downloadSummaries;
        set => SetProperty(ref downloadSummaries, value);
    }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        HutaoInfrastructureClient hutaoInfrastructureClient = serviceProvider.GetRequiredService<HutaoInfrastructureClient>();
        HutaoResponse<StaticResourceSizeInformation> response = await hutaoInfrastructureClient.GetStaticSizeAsync().ConfigureAwait(false);
        if (ResponseValidator.TryValidate(response, serviceProvider, out StaticResourceSizeInformation? sizeInformation))
        {
            await taskContext.SwitchToMainThreadAsync();
            StaticResourceOptions.SizeInformation = sizeInformation;
        }

        return true;
    }

    [Command("NextOrCompleteCommand")]
    private void NextOrComplete()
    {
        ++State;
    }

    private void OnAgreementStateChanged()
    {
        IsNextOrCompleteButtonEnabled = IsTermOfServiceAgreed && IsPrivacyPolicyAgreed && IsIssueReportAgreed && IsOpenSourceLicenseAgreed;
    }

    [SuppressMessage("", "SH003")]
    private async Task DownloadStaticResourceAsync()
    {
        DownloadSummaries = StaticResource
            .GetUnfulfilledCategorySet()
            .Select(category => new DownloadSummary(serviceProvider, category))
            .ToObservableCollection();

        await Parallel.ForEachAsync([.. DownloadSummaries], async (summary, token) =>
        {
            if (await summary.DownloadAndExtractAsync().ConfigureAwait(true))
            {
                taskContext.BeginInvokeOnMainThread(() => DownloadSummaries.Remove(summary));
            }
        }).ConfigureAwait(false);

        StaticResource.FulfillAll();
        UnsafeLocalSetting.Set(SettingKeys.Major1Minor10Revision0GuideState, GuideState.Completed);
        AppInstance.Restart(string.Empty);
    }
}