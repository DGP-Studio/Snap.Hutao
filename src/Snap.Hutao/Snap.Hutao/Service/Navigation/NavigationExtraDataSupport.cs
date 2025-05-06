// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Service.Navigation;

internal static class NavigationExtraDataSupport
{
    public static Task NotifyRecipientAsync(object? target, INavigationCompletionSource data)
    {
        if (data is INavigationExtraData extra)
        {
            return NotifyRecipientAsync(target, extra);
        }

        return Task.CompletedTask;
    }

    public static async Task NotifyRecipientAsync(object? target, INavigationExtraData extra)
    {
        if (target is FrameworkElement frameworkElement)
        {
            await new FrameworkElementLoaded(frameworkElement).WaitAsync().ConfigureAwait(true); // Accessing DataContext requires MainThread

            if (frameworkElement is { DataContext: INavigationRecipient recipient } && extra.Data is not null)
            {
                await recipient.ReceiveAsync(extra).ConfigureAwait(false);
            }
        }

        extra.NotifyNavigationCompleted();
    }

    internal sealed class FrameworkElementLoaded
    {
        private readonly FrameworkElement element;
        private readonly TaskCompletionSource loadTcs = new();

        public FrameworkElementLoaded(FrameworkElement element)
        {
            this.element = element;
            if (!element.IsLoaded)
            {
                element.Loaded += OnLoaded;
            }
        }

        public Task WaitAsync()
        {
            return element.IsLoaded ? Task.CompletedTask : loadTcs.Task;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            element.Loaded -= OnLoaded;
            loadTcs.TrySetResult();
        }
    }
}