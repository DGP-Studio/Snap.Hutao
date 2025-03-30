// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Shell;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Windows.Foundation;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingViewModel : Abstraction.ViewModel, INavigationRecipient
{
    public const string UIGFImportExport = nameof(UIGFImportExport);

    private readonly IShellLinkInterop shellLinkInterop;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private readonly WeakReference<ScrollViewer> weakScrollViewer = new(default!);
    private readonly WeakReference<Border> weakGachaLogBorder = new(default!);

    public partial SettingGeetestViewModel Geetest { get; }

    public partial SettingAppearanceViewModel Appearance { get; }

    public partial SettingStorageViewModel Storage { get; }

    public partial SettingHotKeyViewModel HotKey { get; }

    public partial SettingHomeViewModel Home { get; }

    public partial SettingGameViewModel Game { get; }

    public partial SettingGachaLogViewModel GachaLog { get; }

    public partial SettingWebViewViewModel WebView { get; }

    public void AttachXamlElement(ScrollViewer scrollViewer, Border gachaLogBorder)
    {
        weakScrollViewer.SetTarget(scrollViewer);
        weakGachaLogBorder.SetTarget(gachaLogBorder);
    }

    public async ValueTask<bool> ReceiveAsync(INavigationExtraData data)
    {
        if (!await Initialization.Task.ConfigureAwait(false))
        {
            return false;
        }

        if (!weakScrollViewer.TryGetTarget(out ScrollViewer? scrollViewer) ||
            !weakGachaLogBorder.TryGetTarget(out Border? gachaLogBorder))
        {
            return false;
        }

        if (data.Data is UIGFImportExport)
        {
            await taskContext.SwitchToMainThreadAsync();
            Point point = gachaLogBorder.TransformToVisual(scrollViewer).TransformPoint(new(0, 0));
            scrollViewer.ChangeView(null, point.Y, null, true);
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
        Geetest.IsViewDisposed = true;
        Appearance.IsViewDisposed = true;
        Storage.IsViewDisposed = true;
        HotKey.IsViewDisposed = true;
        Home.IsViewDisposed = true;
        Game.IsViewDisposed = true;
        GachaLog.IsViewDisposed = true;
    }

    [Command("CreateDesktopShortcutCommand")]
    private void CreateDesktopShortcutForElevatedLaunchAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Create desktop shortcut for elevated launch", "SettingViewModel.Command"));

        if (shellLinkInterop.TryCreateDesktopShortcutForElevatedLaunch())
        {
            infoBarService.Success(SH.ViewModelSettingActionComplete);
        }
        else
        {
            infoBarService.Warning(SH.ViewModelSettingCreateDesktopShortcutFailed);
        }
    }
}