// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class LaunchExecutionContext
{
    public LaunchExecutionResult Result { get; } = new();

    public ILogger Logger { get; set; } = default!;

    public LaunchScheme Scheme { get; set; } = default!;

    public LaunchOptions Options { get; set; } = default!;
}