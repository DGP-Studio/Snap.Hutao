// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Snap.Hutao.Core.Graphics;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

internal sealed partial class IdentifyMonitorWindow : Microsoft.UI.Xaml.Window
{
    public IdentifyMonitorWindow(DisplayArea displayArea, int index)
    {
        InitializeComponent();
        Monitor = $"{displayArea.DisplayId.Value:X8}:{index}";

        OverlappedPresenter presenter = OverlappedPresenter.CreateForContextMenu();
        presenter.IsAlwaysOnTop = true;
        presenter.SetBorderAndTitleBar(false, false);
        AppWindow.SetPresenter(presenter);
        PointInt32 point = new(40, 32);
        SizeInt32 size = displayArea.WorkArea.GetSizeInt32().Scale(0.1);
        AppWindow.MoveAndResize(RectInt32Convert.RectInt32(point, size), displayArea);
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
            window.RemoveStyleDialogFrame();
            window.Show();
        }

        await Task.Delay(TimeSpan.FromSeconds(secondsDelay)).ConfigureAwait(true);

        foreach (IdentifyMonitorWindow window in windows)
        {
            window.Close();
        }
    }
}
