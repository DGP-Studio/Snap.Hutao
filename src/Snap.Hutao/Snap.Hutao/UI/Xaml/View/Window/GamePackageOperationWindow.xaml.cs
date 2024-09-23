// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.ViewModel.Game;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Transient)]
internal sealed partial class GamePackageOperationWindow : Microsoft.UI.Xaml.Window,
    IDisposable,
    IXamlWindowExtendContentIntoTitleBar
{
    private readonly IServiceScope scope;

    public GamePackageOperationWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        scope = serviceProvider.CreateScope();

        RectInt32 workArea = DisplayArea.Primary.WorkArea;
        SizeInt32 size = new(workArea.Height, (int)(workArea.Height * 0.75));
        AppWindow.Resize(size.Scale(0.5 * this.GetRasterizationScale()));

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.SetBorderAndTitleBar(false, false);
        }

        this.InitializeController(serviceProvider);
        RootGrid.InitializeDataContext<GamePackageOperationViewModel>(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarAccess { get => DragableGrid; }

    public object DataContext { get => RootGrid.DataContext; }

    public void Dispose()
    {
        scope.Dispose();
    }

    [Command("CloseCommand")]
    private void CloseWindow()
    {
        Close();
    }
}
