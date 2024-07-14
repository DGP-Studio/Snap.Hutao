// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Shell;
using Snap.Hutao.Service.Notification;
using System.Diagnostics;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingViewModel : Abstraction.ViewModel
{
    private readonly SettingDangerousFeatureViewModel dangerousFeatureViewModel;
    private readonly SettingAppearanceViewModel appearanceViewModel;
    private readonly HutaoPassportViewModel hutaoPassportViewModel;
    private readonly SettingGachaLogViewModel gachaLogViewModel;
    private readonly SettingGeetestViewModel geetestViewModel;
    private readonly SettingStorageViewModel storageViewModel;
    private readonly SettingHotKeyViewModel hotKeyViewModel;
    private readonly SettingHomeViewModel homeViewModel;
    private readonly SettingGameViewModel gameViewModel;
    private readonly IShellLinkInterop shellLinkInterop;
    private readonly IInfoBarService infoBarService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    public HutaoPassportViewModel Passport { get => hutaoPassportViewModel; }

    public SettingGeetestViewModel Geetest { get => geetestViewModel; }

    public SettingAppearanceViewModel Appearance { get => appearanceViewModel; }

    public SettingStorageViewModel Storage { get => storageViewModel; }

    public SettingHotKeyViewModel HotKey { get => hotKeyViewModel; }

    public SettingHomeViewModel Home { get => homeViewModel; }

    public SettingGameViewModel Game { get => gameViewModel; }

    public SettingGachaLogViewModel GachaLog { get => gachaLogViewModel; }

    public SettingDangerousFeatureViewModel DangerousFeature { get => dangerousFeatureViewModel; }

    protected override ValueTask<bool> InitializeOverrideAsync()
    {
        Storage.CacheFolderView = new(taskContext, runtimeOptions.LocalCache);
        Storage.DataFolderView = new(taskContext, runtimeOptions.DataFolder);

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

    [Command("RestartAsElevatedCommand")]
    private void RestartAsElevated()
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = $"shell:AppsFolder\\{runtimeOptions.FamilyName}!App",
            UseShellExecute = true,
            Verb = "runas",
        });

        Process.GetCurrentProcess().Kill();
    }
}