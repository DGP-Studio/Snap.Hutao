// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service;

/// <inheritdoc/>
[Injection(InjectAs.Singleton, typeof(IInfoBarService))]
internal class InfoBarService : IInfoBarService
{
    private StackPanel? infoBarStack;

    /// <inheritdoc/>
    public void Initialize(StackPanel container)
    {
        infoBarStack = container;
    }

    /// <inheritdoc/>
    public void Information(string message, int delay = 3000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Informational, null, message, delay);
    }

    /// <inheritdoc/>
    public void Information(string title, string message, int delay = 3000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Informational, title, message, delay);
    }

    /// <inheritdoc/>
    public void Success(string message, int delay = 3000)
    {
        PrepareInfoBarAndShow(InfoBarSeverity.Success, null, message, delay);
    }

    /// <inheritdoc/>
    public void Success(string title, string message, int delay = 3000)
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

    private void PrepareInfoBarAndShow(InfoBarSeverity severity, string? title, string? message, int delay)
    {
        if (infoBarStack is null)
        {
            return;
        }

        infoBarStack.DispatcherQueue.TryEnqueue(() => PrepareInfoBarAndShowInternal(severity, title, message, delay));
    }

    /// <summary>
    /// 此方法应在主线程上运行
    /// </summary>
    /// <param name="severity">严重程度</param>
    /// <param name="title">标题</param>
    /// <param name="message">消息</param>
    /// <param name="delay">关闭延迟</param>
    [SuppressMessage("", "VSTHRD100", Justification = "只能通过 async void 方法使控件在主线程创建")]
    private async void PrepareInfoBarAndShowInternal(InfoBarSeverity severity, string? title, string? message, int delay)
    {
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

    private void OnInfoBarClosed(InfoBar sender, InfoBarClosedEventArgs args)
    {
        Must.NotNull(infoBarStack!).DispatcherQueue.TryEnqueue(() =>
        {
            infoBarStack.Children.Remove(sender);
            sender.Closed -= OnInfoBarClosed;
        });
    }
}
