// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Guide;

/// <summary>
/// 指引视图模型
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Transient)]
internal sealed partial class GuideViewModel : Abstraction.ViewModel
{
    private readonly AppOptions appOptions;
    private string nextOrCompleteButtonText = SH.ViewModelGuideActionNext;
    private bool isNextOrCompleteButtonEnabled = true;
    private NameValue<string>? selectedCulture;
    private bool isTermOfServiceAgreed;
    private bool isPrivacyPolicyAgreed;
    private bool isIssueReportAgreed;
    private bool isOpenSourceLicenseAgreed;

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

    public AppOptions AppOptions { get => appOptions; }

    public NameValue<string>? SelectedCulture
    {
        get => selectedCulture ??= AppOptions.Cultures.FirstOrDefault(c => c.Value == AppOptions.CurrentCulture.Name);
        set
        {
            if (SetProperty(ref selectedCulture, value) && value is not null)
            {
                AppOptions.CurrentCulture = CultureInfo.GetCultureInfo(value.Value);
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

    protected override ValueTask<bool> InitializeUIAsync()
    {
        return ValueTask.FromResult(true);
    }

    [Command("NextOrCompleteCommand")]
    private void NextOrComplete()
    {
        GuideState previousState = (GuideState)State;
        ++State;
    }

    private void OnAgreeSateChanged()
    {
        IsNextOrCompleteButtonEnabled = IsTermOfServiceAgreed && IsPrivacyPolicyAgreed && IsIssueReportAgreed && IsOpenSourceLicenseAgreed;
    }
}