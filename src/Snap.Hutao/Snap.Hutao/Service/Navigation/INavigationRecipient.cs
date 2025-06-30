// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

internal interface INavigationRecipient
{
    /// <summary>
    /// Implement this method to receive extra data during navigation.
    /// </summary>
    /// <param name="data">Extra data</param>
    /// <param name="token">The CancellationToken</param>
    /// <returns>Whether the reception completed successfully</returns>
    ValueTask<bool> ReceiveAsync(INavigationExtraData data, CancellationToken token);
}