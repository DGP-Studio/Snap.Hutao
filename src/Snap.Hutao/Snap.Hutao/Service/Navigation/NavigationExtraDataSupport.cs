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
        if (target is FrameworkElement { DataContext: INavigationRecipient recipient } && extra.Data is not null)
        {
            await recipient.ReceiveAsync(extra).ConfigureAwait(false);
        }

        extra.NotifyNavigationCompleted();
    }
}