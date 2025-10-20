// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using ABI.Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Content;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Win32.UI.Shell;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;
using WinRT;
using WinRT.Interop;
using AppWindowTitleBar = Microsoft.UI.Windowing.AppWindowTitleBar;

namespace Snap.Hutao.UI.Windowing;

[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SA1204")]
internal sealed class XamlWindowController
{
    private readonly Type windowType;
    private readonly Window window;
    private readonly bool hasCustomSystemBackdrop;

    private readonly XamlWindowSubclass subclass;
    private readonly XamlWindowNonRude nonRude;

    public XamlWindowController(Window window, IServiceProvider serviceProvider)
    {
        windowType = window.GetType();
        this.window = window;
        Debug.Assert(serviceProvider is IServiceScope);
        ServiceProvider = serviceProvider;

        // Subclassing and NonRudeHWND are standard infrastructure.
        subclass = new(window);
        nonRude = new(window.GetWindowHandle());

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

            if (xamlWindow.TitleBarCaptionAccess is not TitleBar)
            {
                XamlWindowRegionRects.Update(window);
                xamlWindow.TitleBarCaptionAccess.SizeChanged += OnWindowSizeChanged;
            }
        }

        // Size stuff
        if (window is IXamlWindowHasInitSize xamlWindow2)
        {
            window.AppWindow.Resize(xamlWindow2.InitSize);
        }

        // window.AppWindow.EnablePlacementPersistence(guid, window is MainWindow, default, PlacementPersistenceBehaviorFlags.Default, windowName);
        EnablePlacementRestoration(window);

        window.Content.As<FrameworkElement>().Loading += OnWindowContentLoading;

        window.AppWindow.Show(true);
        window.AppWindow.MoveInZOrderAtTop();

        hasCustomSystemBackdrop = window.SystemBackdrop is not null;

        // SystemBackdrop
        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();
        UpdateSystemBackdrop(appOptions.BackdropType.Value);
        BackdropTypeCallback = appOptions.BackdropType.WithValueChangedCallback(static (value, controller) => controller.UpdateSystemBackdrop(value), this);

        subclass.Initialize();
        window.Closed += OnWindowClosed;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal IServiceProvider ServiceProvider { get; }

    private IObservableProperty<BackdropType>? BackdropTypeCallback { get; }

    public bool TrySetTaskbarProgress(TBPFLAG state, ulong value, ulong maximum)
    {
        try
        {
            subclass.SetTaskbarProgress(state, value, maximum);
            return true;
        }
        catch (Exception ex)
        {
            Debugger.Break();
            SentrySdk.CaptureException(ex);
            return false;
        }
    }

    private static void EnablePlacementRestoration(Window window)
    {
        IObjectReference objRefAppWindowExperimental = Unsafe.As<IWinRTObject>(window.AppWindow).NativeObject.As<IUnknownVftbl>(IAppWindowExperimentalMethods.IID);
        IAppWindowExperimentalMethods.set_PlacementRestorationBehavior(objRefAppWindowExperimental, PlacementRestorationBehavior.All);

        string windowName = TypeNameHelper.GetTypeDisplayName(window);
        byte[] data = CryptographicOperations.HashData(HashAlgorithmName.MD5, Encoding.UTF8.GetBytes(windowName));
        Guid guid = MemoryMarshal.AsRef<Guid>(data);
        IAppWindowExperimentalMethods.set_PersistedStateId(objRefAppWindowExperimental, guid);
    }

    private void OnWindowContentLoading(FrameworkElement element, object e)
    {
        element.Loading -= OnWindowContentLoading;
        element.XamlRoot.ContentIsland.AppData = new XamlContext
        {
            ServiceProvider = ServiceProvider,
        };
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateDebug("WindowClosing", "XamlWindowController", [("type", TypeNameHelper.GetTypeDisplayName(window, false))]));

        if (args.Handled)
        {
            return;
        }

        if (window is IXamlWindowClosedHandler handler)
        {
            handler.OnWindowClosing(out bool cancel);
            if (cancel)
            {
                args.Handled = true;
                return;
            }
        }

        if (!XamlApplicationLifetime.Exiting)
        {
            IServiceProviderIsKeyedService isKeyedService = ServiceProvider.GetRequiredService<IServiceProviderIsKeyedService>();
            ICurrentXamlWindowReference currentXamlWindowReference = isKeyedService.IsKeyedService(typeof(ICurrentXamlWindowReference), windowType)
                ? ServiceProvider.GetRequiredKeyedService<ICurrentXamlWindowReference>(windowType)
                : ServiceProvider.GetRequiredService<ICurrentXamlWindowReference>();

            if (currentXamlWindowReference.Window == window)
            {
                // Only a CurrentWindow can show dialogs
                // Some users might try to close the window while a dialog is showing
                if (ServiceProvider.GetRequiredService<IContentDialogFactory>().IsDialogShowing)
                {
                    args.Handled = true;
                    return;
                }

                currentXamlWindowReference.Window = default!;
            }
        }

        // Detach events
        window.Closed -= OnWindowClosed;
        if (window is IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            xamlWindow.TitleBarCaptionAccess.ActualThemeChanged -= UpdateTitleButtonColor;
            xamlWindow.TitleBarCaptionAccess.SizeChanged -= OnWindowSizeChanged;
        }

        // Dispose components
        subclass.Dispose();
        nonRude.Dispose();

        (window as IXamlWindowClosedHandler)?.OnWindowClosed();

        // Dispose the service scope
        Unsafe.As<IServiceScope>(ServiceProvider).Dispose();
        window.UninitializeController();
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