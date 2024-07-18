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
    IXamlWindowExtendContentIntoTitleBar
{
    public GamePackageOperationWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        RectInt32 workArea = DisplayArea.Primary.WorkArea;
        SizeInt32 size = new(workArea.Height, (int)(workArea.Height * 0.65));
        AppWindow.Resize(size.Scale(0.5));

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
        }

        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarAccess { get => DragableGrid; }

    public void InitializeDataContext(GamePackageOperationViewModel viewModel)
    {
        RootGrid.DataContext = viewModel;
    }

    [Command("CloseCommand")]
    private void CloseWindow()
    {
        Close();
    }
}
