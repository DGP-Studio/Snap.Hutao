// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.Service.Update;

internal sealed class CheckUpdateResult
{
    public CheckUpdateResultKind Kind { get; set; }

    public HutaoPackageInformation? PackageInformation { get; set; }
}