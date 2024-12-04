// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Model;

internal sealed class ElevationStatusResponse
{
    public ElevationStatusResponse(bool isElevated, int processId)
    {
        IsElevated = isElevated;
        ProcessId = processId;
    }

    public bool IsElevated { get; }

    public int ProcessId { get; }
}
