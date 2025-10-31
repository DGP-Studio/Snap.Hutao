// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

[Service(ServiceLifetime.Transient, typeof(IUIGFExportService), Key = UIGFVersion.UIGF41)]
internal sealed partial class UIGF41ExportService : AbstractUIGF40ExportService
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial UIGF41ExportService(IServiceProvider serviceProvider);

    protected override string Version { get; } = "v4.1";
}