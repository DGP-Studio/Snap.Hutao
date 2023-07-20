// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

internal interface IOverseaSupportFactory<TClient>
{
    TClient Create(bool isOversea);

    TClient CreateFor(UserAndUid userAndUid)
    {
        return Create(userAndUid.User.IsOversea);
    }
}