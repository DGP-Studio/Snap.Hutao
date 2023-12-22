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
    private readonly HutaoAsAServiceClient homaAsAServiceClient;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly MainWindow mainWindow;

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

    [Command("ResetGuideStateCommand")]
    private static void ResetGuideState()
    {
        UnsafeLocalSetting.Set(SettingKeys.Major1Minor7Revision0GuideState, GuideState.Language);
    }

    [Command("ExceptionCommand")]
    private static void ThrowTestException()
    {
        Must.NeverHappen();
    }

    [Command("ResetMainWindowSizeCommand")]
    private void ResetMainWindowSize()
    {
        double scale = mainWindow.WindowOptions.GetRasterizationScale();
        mainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1372, 772).Scale(scale));
    }

    [Command("UploadAnnouncementCommand")]
    private async Task UploadAnnouncementAsync()
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
    private async Task CompensationGachaLogServiceTimeAsync()
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