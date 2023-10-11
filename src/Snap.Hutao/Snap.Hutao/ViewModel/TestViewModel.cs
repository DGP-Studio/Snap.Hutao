// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Hutao.HutaoAsAService;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class TestViewModel : Abstraction.ViewModel
{
    private readonly MainWindow mainWindow;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly HutaoAsAServiceClient homaAsAServiceClient;

    private UploadAnnouncement announcement = new();

    public UploadAnnouncement Announcement { get => announcement; set => SetProperty(ref announcement, value); }

    [SuppressMessage("", "CA1822")]
    public bool SuppressMetadataInitialization
    {
        get => LocalSetting.Get(SettingKeys.SuppressMetadataInitialization, false);
        set => LocalSetting.Set(SettingKeys.SuppressMetadataInitialization, value);
    }

    [SuppressMessage("", "CA1822")]
    public bool OverrideElevationRequirement
    {
        get => LocalSetting.Get(SettingKeys.OverrideElevationRequirement, false);
        set => LocalSetting.Set(SettingKeys.OverrideElevationRequirement, value);
    }

    protected override ValueTask<bool> InitializeUIAsync()
    {
        return ValueTask.FromResult(true);
    }

    [Command("ResetGuideStateCommand")]
    private static void ResetGuideState()
    {
        LocalSetting.Set(SettingKeys.Major1Minor7Revision0GuideState, (uint)GuideState.Language);
    }

    [Command("ExceptionCommand")]
    private static void ThrowTestException()
    {
        Must.NeverHappen();
    }

    [Command("ResetMainWindowSizeCommand")]
    private void ResetMainWindowSize()
    {
        mainWindow.AppWindow.Resize(new(1280, 720));
    }

    [Command("UploadAnnouncementCommand")]
    private async void UploadAnnouncementAsync()
    {
        Web.Response.Response response = await homaAsAServiceClient.UploadAnnouncementAsync(Announcement).ConfigureAwait(false);
        if (response.IsOk())
        {
            infoBarService.Success(response.Message);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = new();
        }
    }

    [Command("CompensationGachaLogServiceTimeCommand")]
    private async void CompensationGachaLogServiceTimeAsync()
    {
        Web.Response.Response response = await homaAsAServiceClient.GachaLogCompensationAsync(15).ConfigureAwait(false);
        if (response.IsOk())
        {
            infoBarService.Success(response.Message);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = new();
        }
    }
}