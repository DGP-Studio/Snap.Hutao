// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

[Flags]
internal enum RuntimeResolverLock
{
    Scope = 1,
    Root = 2,
}