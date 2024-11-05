// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Shell;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Windows.Foundation;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingViewModel : Abstraction.ViewModel, INavigationRecipient
{
    public const string UIGFImportExport = "UIGFImportExport";

    private readonly SettingDangerousFeatureViewModel dangerousFeatureViewModel;
    private readonly SettingAppearanceViewModel appearanceViewModel;
    private readonly HutaoPassportViewModel hutaoPassportViewModel;
    private readonly SettingGachaLogViewModel gachaLogViewModel;
    private readonly SettingGeetestViewModel geetestViewModel;
    private readonly SettingStorageViewModel storageViewModel;
    private readonly SettingWebViewViewModel webViewViewModel;
    private readonly SettingHotKeyViewModel hotKeyViewModel;
    private readonly SettingHomeViewModel homeViewModel;
    private readonly SettingGameViewModel gameViewModel;
    private readonly IShellLinkInterop shellLinkInterop;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private ScrollViewer? rootScrollViewer;
    private Border? gachaLogBorder;

    public HutaoPassportViewModel Passport { get => hutaoPassportViewModel; }

    public SettingGeetestViewModel Geetest { get => geetestViewModel; }

    public SettingAppearanceViewModel Appearance { get => appearanceViewModel; }

    public SettingStorageViewModel Storage { get => storageViewModel; }

    public SettingHotKeyViewModel HotKey { get => hotKeyViewModel; }

    public SettingHomeViewModel Home { get => homeViewModel; }

    public SettingGameViewModel Game { get => gameViewModel; }

    public SettingGachaLogViewModel GachaLog { get => gachaLogViewModel; }

    public SettingWebViewViewModel WebView { get => webViewViewModel; }

    public SettingDangerousFeatureViewModel DangerousFeature { get => dangerousFeatureViewModel; }

    public void Initialize(ISettingScrollViewerAccessor accessor)
    {
        rootScrollViewer = accessor.ScrollViewer;
        gachaLogBorder = accessor.GachaLogBorder;
    }

    public async ValueTask<bool> ReceiveAsync(INavigationData data)
    {
        if (!await Initialization.Task.ConfigureAwait(false))
        {
            return false;
        }

        if (rootScrollViewer is null || gachaLogBorder is null)
        {
            return false;
        }

        if (data.Data is UIGFImportExport)
        {
            await taskContext.SwitchToMainThreadAsync();
            Point point = gachaLogBorder.TransformToVisual(rootScrollViewer).TransformPoint(new(0, 0));
            rootScrollViewer.ChangeView(null, point.Y, null, true);
            return true;
        }

        return false;
    }

    protected override ValueTask<bool> LoadOverrideAsync()
    {
        Storage.CacheFolderView = new(taskContext, HutaoRuntime.LocalCache);
        Storage.DataFolderView = new(taskContext, HutaoRuntime.DataFolder);

        return ValueTask.FromResult(true);
    }

    protected override void UninitializeOverride()
    {
        Passport.IsViewDisposed = true;
        Geetest.IsViewDisposed = true;
        Appearance.IsViewDisposed = true;
        Storage.IsViewDisposed = true;
        HotKey.IsViewDisposed = true;
        Home.IsViewDisposed = true;
        Game.IsViewDisposed = true;
        GachaLog.IsViewDisposed = true;
        DangerousFeature.IsViewDisposed = true;
    }

    [Command("CreateDesktopShortcutCommand")]
    private async Task CreateDesktopShortcutForElevatedLaunchAsync()
    {
        if (await shellLinkInterop.TryCreateDesktopShoutcutForElevatedLaunchAsync().ConfigureAwait(false))
        {
            infoBarService.Success(SH.ViewModelSettingActionComplete);
        }
        else
        {
            infoBarService.Warning(SH.ViewModelSettingCreateDesktopShortcutFailed);
        }
    }
}