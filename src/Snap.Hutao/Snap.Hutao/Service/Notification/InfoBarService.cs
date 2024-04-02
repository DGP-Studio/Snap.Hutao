// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Core.Abstraction.Extension;
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

    public void PrepareInfoBarAndShow(Action<IInfoBarOptionsBuilder> configure)
    {
        if (collection is null)
        {
            return;
        }

        PrepareInfoBarAndShowCoreAsync(configure).SafeForget(logger);
    }

    private async ValueTask PrepareInfoBarAndShowCoreAsync(Action<IInfoBarOptionsBuilder> configure)
    {
        IInfoBarOptionsBuilder builder = new InfoBarOptionsBuilder().Configure(configure);

        await taskContext.SwitchToMainThreadAsync();

        InfoBar infoBar = new()
        {
            Severity = builder.Options.Severity,
            Title = builder.Options.Title,
            Message = builder.Options.Message,
            Content = builder.Options.Content,
            IsOpen = true,
            ActionButton = builder.Options.ActionButton,
            Transitions = [new AddDeleteThemeTransition()],
        };

        infoBar.Closed += infobarClosedEventHandler;
        ArgumentNullException.ThrowIfNull(collection);
        collection.Add(infoBar);

        if (builder.Options.MilliSecondsDelay > 0)
        {
            await Delay.FromMilliSeconds(builder.Options.MilliSecondsDelay).ConfigureAwait(true);
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
