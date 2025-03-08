// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.AppNotifications.Builder;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class NotifyIconViewModel : ObservableObject
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly App app;

    public static string Title
    {
        get
        {
            string? title = HutaoRuntime.GetDisplayName();
            ArgumentException.ThrowIfNullOrEmpty(title);
            return title;
        }
    }

    public partial RuntimeOptions RuntimeOptions { get; }

    [Command("OpenCompactWebView2WindowCommand")]
    private static void OpenCompactWebView2Window()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open compact WebView2 window", "NotifyIconViewModel.Command"));

        if (!WindowExtension.IsControllerInitialized<CompactWebView2Window>())
        {
            _ = new CompactWebView2Window();
        }
    }

    [Command("RestartAsElevatedCommand")]
    private void RestartAsElevated()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Restart as elevated", "NotifyIconViewModel.Command"));

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = $"shell:AppsFolder\\{HutaoRuntime.FamilyName}!App",
                UseShellExecute = true,
                Verb = "runas",
            });
        }
        catch (Win32Exception ex)
        {
            // 组或资源的状态不是执行请求操作的正确状态
            if (ex.HResult == HRESULT.E_FAIL)
            {
                try
                {
                    new AppNotificationBuilder().AddText(SH.ViewModelNotifyIconRestartAsElevatedErrorHint).Show();
                    return;
                }
                catch
                {
                }
            }

            throw;
        }

        // Current process will exit in PrivatePipeServer
    }

    [Command("ShowWindowCommand")]
    private void ShowWindow()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Restart as elevated", "NotifyIconViewModel.Command"));

        switch (currentXamlWindowReference.Window)
        {
            case MainWindow mainWindow:
                {
                    // MainWindow is activated, bring to foreground
                    mainWindow.SwitchTo();
                    mainWindow.AppWindow.MoveInZOrderAtTop();
                    return;
                }

            case null:
                {
                    // MainWindow is closed, show it
                    MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                    currentXamlWindowReference.Window = mainWindow;

                    // TODO: Can actually be no any window is initialized
                    mainWindow.SwitchTo();
                    mainWindow.AppWindow.MoveInZOrderAtTop();
                    break;
                }

            default:
                {
                    Window otherWindow = currentXamlWindowReference.Window;
                    otherWindow.SwitchTo();
                    otherWindow.AppWindow.MoveInZOrderAtTop();
                    return;
                }
        }
    }

    [Command("LaunchGameCommand")]
    private async Task LaunchGame()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch Game", "NotifyIconViewModel.Command"));
        if (serviceProvider.GetRequiredService<IAppActivation>() is IAppActivationActionHandlersAccess access)
        {
            await access.HandleLaunchGameActionAsync().ConfigureAwait(false);
        }
    }

    [Command("ExitCommand")]
    private void Exit()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Exit application", "NotifyIconViewModel.Command"));
        app.Exit();
    }

    [Command("OpenScriptingWindowCommand")]
    private void OpenScriptingWindow()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open Scripting Window", "NotifyIconViewModel.Command"));
        _ = serviceProvider.GetRequiredService<ScriptingWindow>();
    }

    [Command("TakeScreenshotCommand")]
    private async Task TakeScreenshotAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Take Window screenshot", "NotifyIconViewModel.Command"));
        INavigationService navigationService = serviceProvider.GetRequiredService<INavigationService>();

        if (currentXamlWindowReference.Window is null)
        {
            return;
        }

        RenderTargetBitmap renderTargetBitmap = new();
        await renderTargetBitmap.RenderAsync(currentXamlWindowReference.Window.Content);

        IBuffer pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
        int width = renderTargetBitmap.PixelWidth;
        int height = renderTargetBitmap.PixelHeight;

        string directory = Path.Combine(HutaoRuntime.GetDataFolderScreenshotFolder(), CultureInfo.CurrentCulture.Name);
        Directory.CreateDirectory(directory);
        string filename = $"{navigationService.CurrentPageType?.Name ?? "None"}_{DateTimeOffset.Now:yyyy.MM.dd_HH.mm.ss}.png";
        using (FileStream fileStream = File.Create(Path.Combine(directory, filename)))
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream.AsRandomAccessStream());
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)width, (uint)height, 72, 72, pixelBuffer.ToArray());
            await encoder.FlushAsync();
        }
    }
}

internal sealed partial class NotifyIconViewModel
{
    public static bool CanTakeScreenshot
    {
        get =>
#if DEBUG || IS_ALPHA_BUILD
            true;
#else
            false;
#endif
    }
}