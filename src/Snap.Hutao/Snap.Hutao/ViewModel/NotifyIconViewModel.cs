﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.View.Window;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class NotifyIconViewModel : ObservableObject
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly RuntimeOptions runtimeOptions;
    private readonly App app;

    public string Title
    {
        [SuppressMessage("", "IDE0027")]
        get
        {
            string name = new StringBuilder()
                .Append("App")
                .AppendIf(runtimeOptions.IsElevated, "Elevated")
#if DEBUG
                .Append("Dev")
#endif
                .Append("NameAndVersion")
                .ToString();

            string? format = SH.GetString(CultureInfo.CurrentCulture, name);
            ArgumentException.ThrowIfNullOrEmpty(format);
            return string.Format(CultureInfo.CurrentCulture, format, runtimeOptions.Version);
        }
    }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    [Command("ShowWindowCommand")]
    private void ShowWindow()
    {
        switch (currentXamlWindowReference.Window)
        {
            case MainWindow mainWindow:
                {
                    // MainWindow is activated, bring to foreground
                    mainWindow.SwitchTo();
                    mainWindow.BringToForeground();
                    return;
                }

            case null:
                {
                    // MainWindow is hided, show it
                    MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                    currentXamlWindowReference.Window = mainWindow;

                    // TODO: Can actually be no any window is initialized
                    mainWindow.SwitchTo();
                    mainWindow.BringToForeground();
                    break;
                }

            case Window otherWindow:
                {
                    otherWindow.SwitchTo();
                    otherWindow.BringToForeground();
                    return;
                }
        }
    }

    [Command("LaunchGameCommand")]
    private async Task LaunchGame()
    {
        if (serviceProvider.GetRequiredService<IAppActivation>() is IAppActivationActionHandlersAccess access)
        {
            await access.HandleLaunchGameActionAsync().ConfigureAwait(false);
        }

        ShowWindow();
    }

    [Command("ExitCommand")]
    private void Exit()
    {
        app.Exit();
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

        // Current process will exit in PrivatePipeServer
    }

    [Command("OpenScriptingWindowCommand")]
    private void OpenScriptingWindow()
    {
        _ = serviceProvider.GetRequiredService<ScriptingWindow>();
    }
}