// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;


[Service(ServiceLifetime.Transient, typeof(IUIGFExportService), Key = UIGFVersion.UIGF40)]
internal sealed partial class UIGF40ExportService : AbstractUIGF40ExportService
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial UIGF40ExportService(IServiceProvider serviceProvider);

    protected override string Version { get; } = "v4.0";
}