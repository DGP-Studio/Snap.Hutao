// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.Setting;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.Guide;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class GuideViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public uint State
    {
        get
        {
            GuideState state = UnsafeLocalSetting.Get(SettingKeys.GuideState, GuideState.Language);

            switch (state)
            {
                case GuideState.Document:
                    IsTermOfServiceAgreed = false;
                    IsPrivacyPolicyAgreed = false;
                    IsIssueReportAgreed = false;
                    IsOpenSourceLicenseAgreed = false;
                    (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionNext, false);
                    break;
                case GuideState.StaticResourceBegin:
                    (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionStaticResourceBegin, false);
                    DownloadStaticResourceAsync().SafeForget();
                    break;
                case GuideState.Completed:
                    (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionComplete, true);
                    break;
                default:
                    (NextOrCompleteButtonText, IsNextOrCompleteButtonEnabled) = (SH.ViewModelGuideActionNext, true);
                    break;
            }

            return (uint)state;
        }

        set
        {
            value = Math.Clamp(value, 0, (uint)GuideState.Completed);
            LocalSetting.Set(SettingKeys.GuideState, value);
            OnPropertyChanged();
        }
    }

    public string NextOrCompleteButtonText { get; set => SetProperty(ref field, value); } = SH.ViewModelGuideActionNext;

    public bool IsNextOrCompleteButtonEnabled { get; set => SetProperty(ref field, value); } = true;

    public partial CultureOptions CultureOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial AppOptions AppOptions { get; }

    public partial StaticResourceOptions StaticResourceOptions { get; }

    public NameCultureInfoValue? SelectedCulture
    {
        get => field ??= Selection.Initialize(CultureOptions.Cultures, CultureOptions.CurrentCulture);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                CultureOptions.CurrentCulture = value.Value;
                AppInstance.Restart(string.Empty);
            }
        }
    }

    public NameValue<Region>? SelectedRegion
    {
        get => field ??= Selection.Initialize(AppOptions.LazyRegions, AppOptions.Region);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.Region = value.Value;
            }
        }
    }

    #region Agreement

    public bool IsTermOfServiceAgreed
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnAgreementStateChanged();
            }
        }
    }

    public bool IsPrivacyPolicyAgreed
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnAgreementStateChanged();
            }
        }
    }

    public bool IsIssueReportAgreed
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnAgreementStateChanged();
            }
        }
    }

    public bool IsOpenSourceLicenseAgreed
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnAgreementStateChanged();
            }
        }
    }

    #endregion

    public ObservableCollection<DownloadSummary>? DownloadSummaries { get; set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HutaoInfrastructureClient hutaoInfrastructureClient = scope.ServiceProvider.GetRequiredService<HutaoInfrastructureClient>();
            HutaoResponse<StaticResourceSizeInformation> response = await hutaoInfrastructureClient.GetStaticSizeAsync().ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out StaticResourceSizeInformation? sizeInformation))
            {
                await taskContext.SwitchToMainThreadAsync();
                StaticResourceOptions.SizeInformation = sizeInformation;
            }

            return true;
        }
    }

    private static ObservableCollection<DownloadSummary> GetUnfulfilledCategoryCollection(IServiceProvider serviceProvider)
    {
        return StaticResource
            .GetUnfulfilledCategorySet()
            .Select(category => new DownloadSummary(serviceProvider, category))
            .ToObservableCollection();
    }

    [Command("NextOrCompleteCommand")]
    private void NextOrComplete()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Increase guide state", "GuideViewModel.Command"));

        ++State;
    }

    [Command("SetDataFolderCommand")]
    private async Task SetDataFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Set data folder path", "GuideViewModel.Command"));

        SettingStorageSetDataFolderOperation operation = new()
        {
            FileSystemPickerInteraction = fileSystemPickerInteraction,
            ContentDialogFactory = contentDialogFactory,
            InfoBarService = infoBarService,
        };

        if (await operation.TryExecuteAsync().ConfigureAwait(false))
        {
            try
            {
                AppInstance.Restart(string.Empty);
            }
            catch (COMException ex)
            {
                infoBarService.Error(ex);
            }
        }
    }

    private void OnAgreementStateChanged()
    {
        IsNextOrCompleteButtonEnabled = IsTermOfServiceAgreed && IsPrivacyPolicyAgreed && IsIssueReportAgreed && IsOpenSourceLicenseAgreed;
    }

    [SuppressMessage("", "SH003")]
    private async Task DownloadStaticResourceAsync()
    {
        DownloadSummaries = GetUnfulfilledCategoryCollection(serviceProvider);

        // Pass a collection copy, so that we can remove element in loop
        await Parallel.ForEachAsync([.. DownloadSummaries], async (summary, token) =>
        {
            if (await summary.DownloadAndExtractAsync().ConfigureAwait(true))
            {
                taskContext.InvokeOnMainThread(() => DownloadSummaries.Remove(summary));
            }
        }).ConfigureAwait(false);

        StaticResource.FulfillAll();
        UnsafeLocalSetting.Set(SettingKeys.GuideState, GuideState.Completed);
        AppInstance.Restart(string.Empty);
    }
}