// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.Process;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.UI.Windowing;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

internal sealed partial class ExceptionWindow : Microsoft.UI.Xaml.Window, INotifyPropertyChanged
{
    private readonly SentryId associatedEventId;

    public ExceptionWindow(SentryId associatedEventId, Exception ex)
    {
        // Message pump will die if we introduce XamlWindowController
        InitializeComponent();
        this.associatedEventId = associatedEventId;
        Exception = ex.ToString();

        AppWindow.Title = "Snap Hutao Exception Report";

        AppWindowTitleBar titleBar = AppWindow.TitleBar;
        titleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        titleBar.ExtendsContentIntoTitleBar = true;

        Closed += (_, _) => ProcessFactory.KillCurrent();

        UpdateDragRectangles();
        DraggableGrid.SizeChanged += (_, _) => UpdateDragRectangles();

        SizeInt32 size = new(800, 400);
        AppWindow.Resize(size.Scale(this.GetRasterizationScale()));
        Bindings.Update();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string TraceId { get => $"trace.id: {associatedEventId}"; }

    public string Exception { get; }

    public string? Comment { get; set => SetProperty(ref field, value); }

    public static void Show(CapturedException capturedException)
    {
        Show(capturedException.Id, capturedException.Exception);
    }

    public static void Show(SentryId id, Exception ex)
    {
        ExceptionWindow window = new(id, ex);
        window.AppWindow.Show(true);
        window.AppWindow.MoveInZOrderAtTop();
    }

    [Command("CloseCommand")]
    private async Task CloseWindow()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Close Window", "ExceptionWindow.Command"));

        Bindings.Update();
        if (!string.IsNullOrWhiteSpace(Comment))
        {
            string? email = await Ioc.Default.GetRequiredService<HutaoUserOptions>().GetActualUserNameAsync().ConfigureAwait(true);
            SentrySdk.CaptureFeedback(Comment, contactEmail: email, associatedEventId: associatedEventId);
        }

        await SentrySdk.FlushAsync().ConfigureAwait(true);
        Close();
    }

    private void UpdateDragRectangles()
    {
        if (!DraggableGrid.IsLoaded)
        {
            return;
        }

        Point position = DraggableGrid.TransformToVisual(Content).TransformPoint(default);
        RectInt32 dragRect = RectInt32Convert.RectInt32(position, DraggableGrid.ActualSize).Scale(this.GetRasterizationScale());
        InputNonClientPointerSource.GetForWindowId(AppWindow.Id).SetRegionRects(NonClientRegionKind.Caption, [dragRect]);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}