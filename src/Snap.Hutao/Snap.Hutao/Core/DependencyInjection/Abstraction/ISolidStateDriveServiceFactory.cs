// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

internal interface ISolidStateDriveServiceFactory<TService>
{
    TService Create(string path);
}
