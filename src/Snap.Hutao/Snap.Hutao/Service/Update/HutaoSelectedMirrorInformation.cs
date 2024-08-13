// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.Service.Update;

internal sealed class HutaoSelectedMirrorInformation
{
    public required Version Version { get; set; }

    public required string Validation { get; set; }

    public required HutaoPackageMirror Mirror { get; set; }
}