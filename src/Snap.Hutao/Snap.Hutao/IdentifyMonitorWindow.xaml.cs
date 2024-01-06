// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core;
using Windows.Graphics;

namespace Snap.Hutao;

internal sealed partial class IdentifyMonitorWindow : Window
{
    public IdentifyMonitorWindow(DisplayArea displayArea, int index)
    {
        InitializeComponent();
        Monitor = $"{displayArea.DisplayId.Value:X8}:{index}";

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.SetBorderAndTitleBar(false, false);
        presenter.IsAlwaysOnTop = true;
        presenter.IsResizable = false;
        AppWindow.SetPresenter(presenter);

        PointInt32 point = new(40, 32);
        SizeInt32 size = StructMarshal.SizeInt32(displayArea.WorkArea).Scale(0.1);
        AppWindow.MoveAndResize(StructMarshal.RectInt32(point, size), displayArea);
    }

    public string Monitor { get; private set; }
}
