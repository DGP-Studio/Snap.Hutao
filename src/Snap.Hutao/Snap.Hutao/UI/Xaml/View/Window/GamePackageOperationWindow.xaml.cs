// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.ViewModel.Game;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Scoped)]
internal sealed partial class GamePackageOperationWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowClosedHandler
{
    private readonly TaskCompletionSource closeTcs = new();

    public GamePackageOperationWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        RectInt32 workArea = DisplayArea.Primary.WorkArea;
        SizeInt32 size = new(workArea.Height, (int)(workArea.Height * 0.75));
        AppWindow.Resize(size.Scale(0.5 * this.GetRasterizationScale()));

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
        }

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        RootGrid.InitializeDataContext<GamePackageOperationViewModel>(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => DraggableGrid; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }

    public Task CloseTask { get => closeTcs.Task; }

    public void OnWindowClosing(out bool cancel)
    {
        cancel = RootGrid.DataContext<GamePackageOperationViewModel>() is { IsFinished: false };
    }

    public void OnWindowClosed()
    {
        closeTcs.TrySetResult();
    }

    internal void HandleProgressUpdate(GamePackageOperationReport status)
    {
        RootGrid.DataContext<GamePackageOperationViewModel>()?.HandleProgressUpdate(status);
    }

    [Command("CloseCommand")]
    private void CloseWindow()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Close Window", "GamePackageOperationWindow.Command"));
        Close();
    }
}