// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

internal static class OverseaSupportFactoryExtension
{
    public static TClient CreateFor<TClient>(this IOverseaSupportFactory<TClient> factory, UserAndUid userAndUid)
    {
        return factory.Create(userAndUid.IsOversea);
    }
}