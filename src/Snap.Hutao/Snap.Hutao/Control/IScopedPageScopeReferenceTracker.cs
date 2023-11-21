// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control;

internal interface IScopedPageScopeReferenceTracker
{
    IServiceScope CreateScope();
}