// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Notification;

/// <inheritdoc/>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInfoBarService))]
internal sealed partial class InfoBarService : IInfoBarService
{
    private readonly ITaskContext taskContext;
    private readonly TaskCompletionSource initializaionCompletionSource;

    private ObservableCollection<InfoBar>? collection;

    /// <inheritdoc/>
    public ObservableCollection<InfoBar> Collection
    {
        get => collection ??= new();
    }

    /// <summary>
    /// 异步等待主窗体加载完成
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>任务</returns>
    public Task WaitInitializationAsync(CancellationToken token = default)
    {
        return initializaionCompletionSource.Task;
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

        PrepareInfoBarAndShowInternalAsync(severity, title, message, delay).SafeForget();
    }

    /// <summary>
    /// 准备信息条并显示
    /// </summary>
    /// <param name="severity">严重程度</param>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    private async Task PrepareInfoBarAndShowInternalAsync(InfoBarSeverity severity, string? title, string? message, int delay)
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

        infoBar.Closed += OnInfoBarClosed;
        collection!.Add(infoBar);

        if (delay > 0)
        {
            await Task.Delay(delay).ConfigureAwait(true);
            collection.Remove(infoBar);
            infoBar.IsOpen = false;
        }
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnInfoBarClosed(InfoBar sender, InfoBarClosedEventArgs args)
    {
        await taskContext.SwitchToMainThreadAsync();

        collection!.Remove(sender);
        sender.Closed -= OnInfoBarClosed;
    }
}
