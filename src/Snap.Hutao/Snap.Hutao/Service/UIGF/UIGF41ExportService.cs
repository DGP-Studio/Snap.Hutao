// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

[GeneratedConstructor(CallBaseConstructor = true)]
[Service(ServiceLifetime.Transient, typeof(IUIGFExportService), Key = UIGFVersion.UIGF41)]
internal sealed partial class UIGF41ExportService : AbstractUIGF40ExportService
{
    protected override string Version { get; } = "v4.1";
}