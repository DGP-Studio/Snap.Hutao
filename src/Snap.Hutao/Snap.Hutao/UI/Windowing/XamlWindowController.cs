// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppNotifications.Builder;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Shell;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;
using WinRT;

namespace Snap.Hutao.UI.Windowing;

[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SA1204")]
internal sealed class XamlWindowController
{
    private readonly Window window;
    private readonly bool hasCustomSystemBackdrop;
    private readonly IServiceProvider serviceProvider;
    private readonly AppOptions appOptions;

    private readonly XamlWindowSubclass subclass;
    private readonly XamlWindowNonRudeHWND windowNonRudeHWND;

    public XamlWindowController(Window window, IServiceProvider serviceProvider)
    {
        this.window = window;
        this.serviceProvider = serviceProvider;

        appOptions = serviceProvider.GetRequiredService<AppOptions>();

        // Subclassing and NonRudeHWND are standard infrastructure.
        subclass = new(window);
        windowNonRudeHWND = new(window.GetWindowHandle());

        window.AppWindow.Title = SH.FormatAppNameAndVersion(HutaoRuntime.Version);
        window.AppWindow.SetIcon(InstalledLocation.GetAbsolutePath("Assets/Logo.ico"));

        // ExtendContentIntoTitleBar
        if (window is IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;
            appTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appTitleBar.ExtendsContentIntoTitleBar = true;

            UpdateTitleButtonColor(default!, default!);
            xamlWindow.TitleBarCaptionAccess.ActualThemeChanged += UpdateTitleButtonColor;

            XamlWindowRegionRects.Update(window);
            xamlWindow.TitleBarCaptionAccess.SizeChanged += OnWindowSizeChanged;
        }

        // Size stuff
        if (window is IXamlWindowHasInitSize xamlWindow2)
        {
            window.AppWindow.Resize(xamlWindow2.InitSize);
        }

        // window.AppWindow.EnablePlacementPersistence(guid, window is MainWindow, default, PlacementPersistenceBehaviorFlags.Default, windowName);
        EnablePlacementPersistence(window);

        window.AppWindow.Show(true);
        window.AppWindow.MoveInZOrderAtTop();

        hasCustomSystemBackdrop = window.SystemBackdrop is not null;

        // SystemBackdrop
        UpdateSystemBackdrop(appOptions.BackdropType);

        appOptions.PropertyChanged += OnAppOptionsPropertyChanged;

        subclass.Initialize();
        window.Closed += OnWindowClosed;
    }

    private static void EnablePlacementPersistence(Window window)
    {
        // Microsoft.UI.Windowing.dll
        IAppWindowExperimental appWindowExperimental = ((IWinRTObject)window.AppWindow).NativeObject.AsInterface<IAppWindowExperimental>();
        appWindowExperimental.SetPlacementRestorationBehavior(65535); // PlacementRestorationBehavior.Default

        string windowName = TypeNameHelper.GetTypeDisplayName(window);
        byte[] data = CryptographicOperations.HashData(HashAlgorithmName.MD5, Encoding.UTF8.GetBytes(windowName));
        Guid guid = MemoryMarshal.AsRef<Guid>(data);

        ObjectReferenceValue guidValue = ABI.System.Nullable<Guid>.CreateMarshaler2(guid);
        try
        {
            appWindowExperimental.SetPersistedStateId(ABI.System.Nullable<Guid>.CreateMarshaler2(guid).GetAbi());
        }
        finally
        {
            guidValue.Dispose();
        }
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        if (XamlApplicationLifetime.LaunchedWithNotifyIcon && !XamlApplicationLifetime.Exiting)
        {
            ICurrentXamlWindowReference currentXamlWindowReference = serviceProvider.GetRequiredService<ICurrentXamlWindowReference>();
            if (currentXamlWindowReference.Window == window)
            {
                // Only a CurrentWindow can show dialogs
                // Some users might try to close the window while a dialog is showing
                // If not LaunchedWithNotifyIcon: the process should be terminated anyway.
                if (serviceProvider.GetRequiredService<IContentDialogFactory>().IsDialogShowing)
                {
                    args.Handled = true;
                    return;
                }

                currentXamlWindowReference.Window = default!;

                // NotifyIcon Promotion should only be checked when it's a CurrentWindow
                if (!NotifyIcon.IsPromoted(serviceProvider))
                {
                    try
                    {
                        new AppNotificationBuilder()
                            .AddText(SH.CoreWindowingNotifyIconPromotedHint)
                            .Show();
                    }
                    catch
                    {
                        // Ignore
                    }
                }
            }

            GC.Collect(GC.MaxGeneration);
        }

        // Detach events
        window.Closed -= OnWindowClosed;
        appOptions.PropertyChanged -= OnAppOptionsPropertyChanged;
        if (window is IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            xamlWindow.TitleBarCaptionAccess.ActualThemeChanged -= UpdateTitleButtonColor;
            xamlWindow.TitleBarCaptionAccess.SizeChanged -= OnWindowSizeChanged;
        }

        // Dispose components
        subclass?.Dispose();
        windowNonRudeHWND?.Dispose();

        if (window is IXamlWindowClosedHandler xamlWindowClosed)
        {
            xamlWindowClosed.OnWindowClosed();
        }

        window.UninitializeController();
    }

    private void OnAppOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not AppOptions options)
        {
            return;
        }

        _ = e.PropertyName switch
        {
            nameof(AppOptions.BackdropType) => UpdateSystemBackdrop(options.BackdropType),
            _ => false,
        };
    }

    private bool UpdateSystemBackdrop(BackdropType backdropType)
    {
        if (hasCustomSystemBackdrop)
        {
            return true;
        }

        window.SystemBackdrop = backdropType switch
        {
            BackdropType.Transparent => new TransparentBackdrop(),
            BackdropType.MicaAlt => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            BackdropType.Mica => new MicaBackdrop { Kind = MicaKind.Base },
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            _ => null,
        };

        return true;
    }

    private void UpdateTitleButtonColor(FrameworkElement discardElement, object e)
    {
        if (window is not IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            return;
        }

        AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;

        appTitleBar.ButtonBackgroundColor = Colors.Transparent;
        appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        bool isDarkMode = ThemeHelper.IsDarkMode(xamlWindow.TitleBarCaptionAccess.ActualTheme);

        Color systemBaseLowColor = SystemColors.BaseLowColor(isDarkMode);
        appTitleBar.ButtonHoverBackgroundColor = systemBaseLowColor;

        Color systemBaseMediumLowColor = SystemColors.BaseMediumLowColor(isDarkMode);
        appTitleBar.ButtonPressedBackgroundColor = systemBaseMediumLowColor;

        // The Foreground doesn't accept Alpha channel. So we translate it to gray.
        byte light = (byte)((systemBaseMediumLowColor.R + systemBaseMediumLowColor.G + systemBaseMediumLowColor.B) / 3);
        byte result = (byte)(systemBaseMediumLowColor.A / 255.0 * light);
        appTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, result, result, result);

        Color systemBaseHighColor = SystemColors.BaseHighColor(isDarkMode);
        appTitleBar.ButtonForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonHoverForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonPressedForegroundColor = systemBaseHighColor;
    }

    private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        XamlWindowRegionRects.Update(window);
    }
}

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SYSLIB1096")]
[Guid("04db96c7-deb6-5be4-bfdc-1bc0361c8a12")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
file unsafe interface IAppWindowExperimental
{
    [PreserveSig]
    HRESULT GetIids(/* Ignored */);

    [PreserveSig]
    HRESULT GetRuntimeClassName(/* Ignored */);

    [PreserveSig]
    HRESULT GetTrustLevel(/* Ignored */);

    [PreserveSig]
    HRESULT GetPersistedStateId(/* Ignored */);

    // Api::IAppWindowExperimental::ExportAdapter$::put_PersistedStateId(Windows::Foundation::IReference<_GUID> *)
    [PreserveSig]
    HRESULT SetPersistedStateId(/* Windows::Foundation::IReference<_GUID>* */ nint value);

    [PreserveSig]
    HRESULT GetPlacementRestorationBehavior(/* Ignored */);

    // Api::IAppWindowExperimental::ExportAdapter$::put_PlacementRestorationBehavior(Microsoft::UI::Windowing::PlacementRestorationBehavior)
    [PreserveSig]
    HRESULT SetPlacementRestorationBehavior(/* Microsoft::UI::Windowing::PlacementRestorationBehavior */ int value);

    [PreserveSig]
    HRESULT GetCurrentPlacement(/* Ignored */);

    [PreserveSig]
    HRESULT SaveCurrentPlacement(/* Ignored */);

    [PreserveSig]
    HRESULT SetCurrentPlacement(/* Ignored */);
}