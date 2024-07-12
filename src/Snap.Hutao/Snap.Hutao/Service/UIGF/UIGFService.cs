// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IUIGFService))]
internal sealed partial class UIGFService : IUIGFService
{
    private readonly IServiceProvider serviceProvider;

    public ValueTask<bool> ExportAsync(UIGFExportOptions exportOptions, CancellationToken token)
    {
        IUIGFExportService exportService = serviceProvider.GetRequiredKeyedService<IUIGFExportService>(UIGFVersion.UIGF40);
        return exportService.ExportAsync(exportOptions, token);
    }
}