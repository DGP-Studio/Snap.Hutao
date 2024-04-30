// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.Update;

internal sealed class LaunchUpdaterResult
{
    public bool IsSuccess { get; set; }

    public Process? Process { get; set; }
}