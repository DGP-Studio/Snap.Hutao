// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Singleton)]
internal sealed partial class GuideWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowRectPersisted,
    IXamlWindowSubclassMinMaxInfoHandler
{
    private const int MinWidth = 1000;
    private const int MinHeight = 650;

    private const int MaxWidth = 1200;
    private const int MaxHeight = 800;

    public GuideWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
        }

        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarAccess { get => DragableGrid; }

    public string PersistRectKey { get => SettingKeys.GuideWindowRect; }

    public string PersistScaleKey { get => SettingKeys.GuideWindowScale; }

    public SizeInt32 InitSize { get; } = new(MinWidth, MinHeight);

    public SizeInt32 MinSize { get; } = new(MinWidth, MinHeight);

    public void HandleMinMaxInfo(ref MINMAXINFO info, double scalingFactor)
    {
        info.ptMinTrackSize.x = (int)Math.Max(MinWidth * scalingFactor, info.ptMinTrackSize.x);
        info.ptMinTrackSize.y = (int)Math.Max(MinHeight * scalingFactor, info.ptMinTrackSize.y);
        info.ptMaxTrackSize.x = (int)Math.Min(MaxWidth * scalingFactor, info.ptMaxTrackSize.x);
        info.ptMaxTrackSize.y = (int)Math.Min(MaxHeight * scalingFactor, info.ptMaxTrackSize.y);
    }
}