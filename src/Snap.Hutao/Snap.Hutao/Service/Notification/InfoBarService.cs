// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;
using Windows.Foundation;

namespace Snap.Hutao.Service.Notification;

/// <inheritdoc/>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(IInfoBarService))]
internal sealed class InfoBarService : IInfoBarService
{
    private readonly ILogger<InfoBarService> logger;
    private readonly ITaskContext taskContext;

    private readonly TypedEventHandler<InfoBar, InfoBarClosedEventArgs> infobarClosedEventHandler;

    private ObservableCollection<InfoBar>? collection;

    public InfoBarService(IServiceProvider serviceProvider)
    {
        logger = serviceProvider.GetRequiredService<ILogger<InfoBarService>>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        infobarClosedEventHandler = OnInfoBarClosed;
    }

    /// <inheritdoc/>
    public ObservableCollection<InfoBar> Collection
    {
        get => collection ??= [];
    }

    public void PrepareInfoBarAndShow(InfoBarSeverity severity, string? title, string? message, int delay)
    {
        if (collection is null)
        {
            return;
        }

        PrepareInfoBarAndShowCoreAsync(severity, title, message, delay).SafeForget(logger);
    }

    private async ValueTask PrepareInfoBarAndShowCoreAsync(InfoBarSeverity severity, string? title, string? message, int delay)
    {
        await taskContext.SwitchToMainThreadAsync();

        InfoBar infoBar = new()
        {
            Severity = severity,
            Title = title,
            Message = message,
            IsOpen = true,
            Transitions = [new AddDeleteThemeTransition()],
        };

        infoBar.Closed += infobarClosedEventHandler;
        ArgumentNullException.ThrowIfNull(collection);
        collection.Add(infoBar);

        if (delay > 0)
        {
            await Delay.FromMilliSeconds(delay).ConfigureAwait(true);
            collection.Remove(infoBar);
            infoBar.IsOpen = false;
        }
    }

    private void OnInfoBarClosed(InfoBar sender, InfoBarClosedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(collection);
        taskContext.BeginInvokeOnMainThread(() => collection.Remove(sender));
        sender.Closed -= infobarClosedEventHandler;
    }
}