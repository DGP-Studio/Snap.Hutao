// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

internal interface INavigationExtraData : INavigationCompletionSource
{
    object? Data { get; set; }

    void NotifyNavigationCompleted();

    void NotifyNavigationException(Exception exception);
}