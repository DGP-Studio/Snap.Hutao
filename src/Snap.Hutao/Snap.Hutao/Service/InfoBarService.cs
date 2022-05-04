// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.Threading;
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

    /// <inheritdoc/>
    public void Show(InfoBar infoBar, int delay = 0)
    {
        Must.NotNull(infoBarStack!).DispatcherQueue.TryEnqueue(ShowInfoBarOnUIThreadAsync(infoBarStack, infoBar, delay).Forget);
    }

    private async Task ShowInfoBarOnUIThreadAsync(StackPanel stack, InfoBar infoBar, int delay)
    {
        infoBar.Closed += OnInfoBarClosed;
        stack.Children.Add(infoBar);

        if (delay > 0)
        {
            await Task.Delay(delay);
            infoBar.IsOpen = false;
        }
    }

    private void PrepareInfoBarAndShow(InfoBarSeverity severity, string? title, string? message, int delay)
    {
        InfoBar infoBar = new()
        {
            Severity = severity,
            Title = title,
            Message = message,
            IsOpen = true,
        };

        Show(infoBar, delay);
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
