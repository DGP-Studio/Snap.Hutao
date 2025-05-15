// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Input;
using Snap.Hutao.UI.Input.LowLevel;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.ViewModel.Overlay;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Transient)]
internal sealed partial class LaunchExecutionOverlayWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowMouseWheelHandler,
    IXamlWindowClosedHandler,
    IWindowNeedEraseBackground
{
    private readonly LowLevelKeyOptions lowLevelKeyOptions;
    private readonly ITaskContext taskContext;

    public LaunchExecutionOverlayWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        AppWindow.Title = "SnapHutaoLaunchExecutionOverlay";
        AppWindow.SafeIsShowInSwitchers(false);

        SizeInt32 size = ScaledSizeInt32.CreateForWindow(386, 56, this);

        // Thanks to @Scighost for the following code
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.SetBorderAndTitleBar(false, false);
            presenter.PreferredMinimumWidth = size.Width;
            presenter.PreferredMinimumHeight = size.Height;
            presenter.PreferredMaximumWidth = size.Height;
            presenter.PreferredMaximumHeight = size.Height;
        }

        this.AddExtendedStyleLayered();
        this.RemoveStyleOverlappedWindow();

        SystemBackdrop = new TransparentBackdrop();

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        RootView.InitializeDataContext<OverlayViewModel>(scope.ServiceProvider);
        lowLevelKeyOptions = scope.ServiceProvider.GetRequiredService<LowLevelKeyOptions>();
        taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();

        AppWindow.Resize(size);
        if (HideByHotKey)
        {
            AppWindow.Hide();
        }

        InputLowLevelKeyboardSource.Initialize();
        InputLowLevelKeyboardSource.KeyDown += OnLowLevelKeyDown;
    }

    public FrameworkElement TitleBarCaptionAccess { get => RootBorder; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }

    public bool PreventClose { get; set; } = true;

    public bool HideByHotKey { get => !LocalSetting.Get(SettingKeys.OverlayWindowIsVisible, true); }

    public bool HideByEvent { get; set; }

    public void OnMouseWheel(ref readonly PointerPointProperties data)
    {
        RootView.DataContext<OverlayViewModel>()?.HandleMouseWheel(data.Delta / 120);
    }

    public void OnWindowClosing(out bool cancel)
    {
        cancel = PreventClose;
    }

    public void OnWindowClosed()
    {
        InputLowLevelKeyboardSource.KeyDown -= OnLowLevelKeyDown;
        InputLowLevelKeyboardSource.Uninitialize();
    }

    private void OnLowLevelKeyDown(LowLevelKeyEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        VIRTUAL_KEY key = (VIRTUAL_KEY)args.Data.vkCode;
        if (key is VIRTUAL_KEY.VK__none_)
        {
            // Skipping VK__none_ handling
            return;
        }

        if (key == lowLevelKeyOptions.OverlayHideKey.Value)
        {
            if (HideByEvent)
            {
                return;
            }

            taskContext.InvokeOnMainThread(() =>
            {
                if (AppWindow.IsVisible)
                {
                    AppWindow.Hide();
                    LocalSetting.Set(SettingKeys.OverlayWindowIsVisible, false);
                }
                else
                {
                    AppWindow.Show(false);
                    AppWindow.MoveInZOrderAtTop();
                    LocalSetting.Set(SettingKeys.OverlayWindowIsVisible, true);
                }
            });
        }
    }
}