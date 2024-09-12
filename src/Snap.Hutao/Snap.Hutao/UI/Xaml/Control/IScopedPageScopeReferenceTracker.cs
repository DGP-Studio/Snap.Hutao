﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control;

internal interface IScopedPageScopeReferenceTracker : IDisposable
{
    IServiceScope CreateScope();
}