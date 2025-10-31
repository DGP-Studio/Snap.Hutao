// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

[Service(ServiceLifetime.Transient, typeof(IUIGFImportService), Key = UIGFVersion.UIGF41)]
internal sealed partial class UIGF41ImportService : AbstractUIGF40ImportService
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial UIGF41ImportService(IServiceProvider serviceProvider);
}