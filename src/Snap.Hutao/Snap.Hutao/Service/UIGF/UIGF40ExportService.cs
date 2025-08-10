// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient, typeof(IUIGFExportService), Key = UIGFVersion.UIGF40)]
internal sealed partial class UIGF40ExportService : AbstractUIGF40ExportService
{
    protected override string Version { get; } = "v4.0";
}