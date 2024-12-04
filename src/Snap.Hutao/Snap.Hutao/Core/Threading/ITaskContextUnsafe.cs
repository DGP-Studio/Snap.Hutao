// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace Snap.Hutao.Core.Threading;

internal interface ITaskContextUnsafe
{
    DispatcherQueue DispatcherQueue { get; }
}