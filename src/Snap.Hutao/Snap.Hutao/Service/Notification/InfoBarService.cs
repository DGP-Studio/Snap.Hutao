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
        get => collection ??= new();
    }

    /// <inheritdoc/>
    public void Information(string message, int delay = 5000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Informational, null, message, delay);
    }

    /// <inheritdoc/>
    public void Information(string title, string message, int delay = 5000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Informational, title, message, delay);
    }

    /// <inheritdoc/>
    public void Success(string message, int delay = 5000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Success, null, message, delay);
    }

    /// <inheritdoc/>
    public void Success(string title, string message, int delay = 5000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Success, title, message, delay);
    }

    /// <inheritdoc/>
    public void Warning(string message, int delay = 30000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Warning, null, message, delay);
    }

    /// <inheritdoc/>
    public void Warning(string title, string message, int delay = 30000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Warning, title, message, delay);
    }

    /// <inheritdoc/>
    public void Error(string message, int delay = 0)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Error, null, message, delay);
    }

    /// <inheritdoc/>
    public void Error(string title, string message, int delay = 0)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Error, title, message, delay);
    }

    /// <inheritdoc/>
    public void Error(Exception ex, int delay = 0)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Error, ex.GetType().Name, ex.Message, delay);
    }

    /// <inheritdoc/>
    public void Error(Exception ex, string title, int delay = 0)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Error, ex.GetType().Name, $"{title}\n{ex.Message}", delay);
    }

    private void PrepareInfoBarAndShow(InfoBarSeverity severity, string? title, string? message, int delay)
    {
        if (collection is null)
        {
            return;
        }

        PrepareInfoBarAndShowInternalAsync(severity, title, message, delay).SafeForget(logger);
    }

    /// <summary>
    /// 准备信息条并显示
    /// </summary>
    /// <param name="severity">严重程度</param>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    private async ValueTask PrepareInfoBarAndShowInternalAsync(InfoBarSeverity severity, string? title, string? message, int delay)
    {
        await taskContext.SwitchToMainThreadAsync();

        InfoBar infoBar = new()
        {
            Severity = severity,
            Title = title,
            Message = message,
            IsOpen = true,
            Transitions = new() { new AddDeleteThemeTransition() },
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
        taskContext.InvokeOnMainThread(() => collection.Remove(sender));
        sender.Closed -= infobarClosedEventHandler;
    }
}
