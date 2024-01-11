﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Guide;

/// <summary>
/// 指引视图模型
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class GuideViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly CultureOptions cultureOptions;
    private readonly RuntimeOptions runtimeOptions;

    private string nextOrCompleteButtonText = SH.ViewModelGuideActionNext;
    private bool isNextOrCompleteButtonEnabled = true;
    private NameValue<CultureInfo>? selectedCulture;
    private bool isTermOfServiceAgreed;
    private bool isPrivacyPolicyAgreed;
    private bool isIssueReportAgreed;
    private bool isOpenSourceLicenseAgreed;
    private ObservableCollection<DownloadSummary>? downloadSummaries;

    public uint State
    {
        get
        {
            uint value = LocalSetting.Get(SettingKeys.Major1Minor7Revision0GuideState, 0U);
            GuideState state = (GuideState)value;

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
                DownloadStaticResourceAsync().SafeForget();
            }
            else if (state is GuideState.Completed)
            {
                (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionComplete, true);
            }
            else
            {
                (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionNext, true);
            }

            return value;
        }

        set
        {
            LocalSetting.Set(SettingKeys.Major1Minor7Revision0GuideState, value);
            OnPropertyChanged();
        }
    }

    public string NextOrCompleteButtonText { get => nextOrCompleteButtonText; set => SetProperty(ref nextOrCompleteButtonText, value); }

    public bool IsNextOrCompleteButtonEnabled { get => isNextOrCompleteButtonEnabled; set => SetProperty(ref isNextOrCompleteButtonEnabled, value); }

    public CultureOptions CultureOptions { get => cultureOptions; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

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

    public bool IsTermOfServiceAgreed
    {
        get => isTermOfServiceAgreed; set
        {
            if (SetProperty(ref isTermOfServiceAgreed, value))
            {
                OnAgreeSateChanged();
            }
        }
    }

    public bool IsPrivacyPolicyAgreed
    {
        get => isPrivacyPolicyAgreed; set
        {
            if (SetProperty(ref isPrivacyPolicyAgreed, value))
            {
                OnAgreeSateChanged();
            }
        }
    }

    public bool IsIssueReportAgreed
    {
        get => isIssueReportAgreed; set
        {
            if (SetProperty(ref isIssueReportAgreed, value))
            {
                OnAgreeSateChanged();
            }
        }
    }

    public bool IsOpenSourceLicenseAgreed
    {
        get => isOpenSourceLicenseAgreed; set
        {
            if (SetProperty(ref isOpenSourceLicenseAgreed, value))
            {
                OnAgreeSateChanged();
            }
        }
    }

    /// <summary>
    /// 下载信息
    /// </summary>
    public ObservableCollection<DownloadSummary>? DownloadSummaries
    {
        get => downloadSummaries;
        set => SetProperty(ref downloadSummaries, value);
    }

    [Command("NextOrCompleteCommand")]
    private void NextOrComplete()
    {
        ++State;
    }

    private void OnAgreeSateChanged()
    {
        IsNextOrCompleteButtonEnabled = IsTermOfServiceAgreed && IsPrivacyPolicyAgreed && IsIssueReportAgreed && IsOpenSourceLicenseAgreed;
    }

    private async ValueTask DownloadStaticResourceAsync()
    {
        DownloadSummaries = StaticResource
            .GetUnfulfilledCategorySet()
            .Select(category => new DownloadSummary(serviceProvider, category))
            .ToObservableCollection();

        await Parallel.ForEachAsync([..DownloadSummaries], async (summary, token) =>
        {
            if (await summary.DownloadAndExtractAsync().ConfigureAwait(false))
            {
                taskContext.BeginInvokeOnMainThread(() => DownloadSummaries.Remove(summary));
            }
        }).ConfigureAwait(false);

        StaticResource.FulfillAll();
        UnsafeLocalSetting.Set(SettingKeys.Major1Minor7Revision0GuideState, GuideState.Completed);
        AppInstance.Restart(string.Empty);
    }
}