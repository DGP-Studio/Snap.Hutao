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
    public ScriptingWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        RootGrid.InitializeDataContext<ScriptingViewModel>(serviceProvider);
        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => DragableGrid; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get; } = [];

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(800, 500, this); }

    public SizeInt32 MinSize { get => ScaledSizeInt32.CreateForWindow(600, 200, this); }
}