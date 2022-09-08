// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service;

/// <inheritdoc/>
[Injection(InjectAs.Singleton, typeof(IInfoBarService))]
internal class InfoBarService : IInfoBarService
{
    private readonly TaskCompletionSource initializaionCompletionSource = new();
    private StackPanel? infoBarStack;

    /// <inheritdoc/>
    public void Initialize(StackPanel container)
    {
        infoBarStack = container;
        initializaionCompletionSource.TrySetResult();
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
    public void Warning(string message, int delay = 0)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Warning, null, message, delay);
    }

    /// <inheritdoc/>
    public void Warning(string title, string message, int delay = 0)
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

    [SuppressMessage("", "VSTHRD100")]
    private async void PrepareInfoBarAndShow(InfoBarSeverity severity, string? title, string? message, int delay)
    {
        if (infoBarStack is null)
        {
            return;
        }

        await PrepareInfoBarAndShowInternalAsync(severity, title, message, delay).ConfigureAwait(false);
    }

    /// <summary>
    /// 此方法应在主线程上运行
    /// </summary>
    /// <param name="severity">严重程度</param>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    private async Task PrepareInfoBarAndShowInternalAsync(InfoBarSeverity severity, string? title, string? message, int delay)
    {
        await ThreadHelper.SwitchToMainThreadAsync();

        InfoBar infoBar = new()
        {
            Severity = severity,
            Title = title,
            Message = message,
            IsOpen = true,
        };

        infoBar.Closed += OnInfoBarClosed;
        Must.NotNull(infoBarStack!).Children.Add(infoBar);

        if (delay > 0)
        {
            await Task.Delay(delay);
            infoBar.IsOpen = false;
        }
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnInfoBarClosed(InfoBar sender, InfoBarClosedEventArgs args)
    {
        await ThreadHelper.SwitchToMainThreadAsync();

        Must.NotNull(infoBarStack!).Children.Remove(sender);
        sender.Closed -= OnInfoBarClosed;
    }
}
