// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal enum CallSiteKind
{
    Factory,
    Constructor,
    Constant,
    IEnumerable,
    ServiceProvider,
}