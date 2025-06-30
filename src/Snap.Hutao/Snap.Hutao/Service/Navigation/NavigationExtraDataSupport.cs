// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Service.Navigation;

internal static class NavigationExtraDataSupport
{
    public static Task<bool> NotifyRecipientAsync(object? target, INavigationCompletionSource data, CancellationToken token)
    {
        if (data is INavigationExtraData extra)
        {
            return NotifyRecipientAsync(target, extra, token);
        }

        return token.IsCancellationRequested ? Task.FromCanceled<bool>(token) : Task.FromResult(false);
    }

    public static async Task<bool> NotifyRecipientAsync(object? target, INavigationExtraData extra, CancellationToken token)
    {
        bool result = false;
        if (target is FrameworkElement frameworkElement)
        {
            await new FrameworkElementLoaded(frameworkElement).WaitAsync().ConfigureAwait(true);

            // Accessing DataContext requires MainThread
            if (frameworkElement is { DataContext: INavigationRecipient recipient } && extra.Data is not null)
            {
                result = await recipient.ReceiveAsync(extra, token).ConfigureAwait(false);
            }
        }

        extra.NotifyNavigationCompleted();
        return result;
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