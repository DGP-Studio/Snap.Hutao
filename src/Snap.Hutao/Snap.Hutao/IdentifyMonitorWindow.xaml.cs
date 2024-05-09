// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Win32;
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

    public static async ValueTask IdentifyAllMonitorsAsync(int secondsDelay)
    {
        List<IdentifyMonitorWindow> windows = [];

        // TODO: the order here is not sync with unity.
        IReadOnlyList<DisplayArea> displayAreas = DisplayArea.FindAll();
        for (int i = 0; i < displayAreas.Count; i++)
        {
            windows.Add(new IdentifyMonitorWindow(displayAreas[i], i + 1));
        }

        foreach (IdentifyMonitorWindow window in windows)
        {
            window.Activate();
        }

        await Delay.FromSeconds(secondsDelay).ConfigureAwait(true);

        foreach (IdentifyMonitorWindow window in windows)
        {
            window.Close();
        }
    }
}
