// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Core.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Graphics;

namespace Snap.Hutao;

[HighQuality]
[Injection(InjectAs.Transient)]
internal sealed partial class LaunchGameWindow : Window,
    IDisposable,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize,
    IXamlWindowSubclassMinMaxInfoHandler
{
    private const int MinWidth = 240;
    private const int MinHeight = 240;

    private const int MaxWidth = 320;
    private const int MaxHeight = 320;

    private readonly IServiceScope scope;

    public LaunchGameWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        scope = serviceProvider.CreateScope();

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
        }

        this.InitializeController(serviceProvider);
        RootGrid.InitializeDataContext<LaunchGameViewModel>(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarAccess { get => DragableGrid; }

    public SizeInt32 InitSize { get; } = new(MaxWidth, MaxHeight);

    public SizeInt32 MinSize { get; } = new(MinWidth, MinHeight);

    /// <inheritdoc/>
    public void Dispose()
    {
        scope.Dispose();
    }

    /// <inheritdoc/>
    public unsafe void HandleMinMaxInfo(ref MINMAXINFO info, double scalingFactor)
    {
        info.ptMinTrackSize.x = (int)Math.Max(MinWidth * scalingFactor, info.ptMinTrackSize.x);
        info.ptMinTrackSize.y = (int)Math.Max(MinHeight * scalingFactor, info.ptMinTrackSize.y);
        info.ptMaxTrackSize.x = (int)Math.Min(MaxWidth * scalingFactor, info.ptMaxTrackSize.x);
        info.ptMaxTrackSize.y = (int)Math.Min(MaxHeight * scalingFactor, info.ptMaxTrackSize.y);
    }
}