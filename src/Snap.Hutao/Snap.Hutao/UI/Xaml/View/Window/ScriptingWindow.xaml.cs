// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.ViewModel.Scripting;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Transient)]
internal sealed partial class ScriptingWindow : Microsoft.UI.Xaml.Window, IXamlWindowExtendContentIntoTitleBar, IXamlWindowHasInitSize
{
    private const int MinWidth = 600;
    private const int MinHeight = 200;

    public ScriptingWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        RootGrid.InitializeDataContext<ScriptingViewModel>(serviceProvider);
        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarAccess { get => DragableGrid; }

    public SizeInt32 InitSize { get; } = new(800, 500);

    public SizeInt32 MinSize { get; } = new(MinWidth, MinHeight);
}