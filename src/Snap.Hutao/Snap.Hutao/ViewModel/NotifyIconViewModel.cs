// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Windowing;
using System.Globalization;
using System.Text;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class NotifyIconViewModel : ObservableObject
{
    private readonly RuntimeOptions runtimeOptions;
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
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

    [Command("ShowMainWindowCommand")]
    private void ShowMainWindow()
    {
        switch (currentXamlWindowReference.Window)
        {
            case MainWindow mainWindow:
                {
                    // MainWindow is activated, bring to foreground
                    mainWindow.Show();
                    mainWindow.WindowOptions.BringToForeground();
                    return;
                }

            case null:
                {
                    // MainWindow is hided, show it
                    MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                    currentXamlWindowReference.Window = mainWindow;

                    // TODO: Can actually be no any window is initialized
                    mainWindow.Show();
                    break;
                }

            case Window otherWindow:
                {
                    if (otherWindow is IXamlWindowOptionsSource optionsSource)
                    {
                        otherWindow.Show();
                        optionsSource.WindowOptions.BringToForeground();
                    }

                    return;
                }
        }
    }

    [Command("ExitCommand")]
    private void Exit()
    {
        app.Exit();
    }
}