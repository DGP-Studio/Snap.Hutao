// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Log;

internal sealed class HutaoLog
{
    public string Id { get; set; } = default!;

    public long Time { get; set; }

    public string Info { get; set; } = default!;
}