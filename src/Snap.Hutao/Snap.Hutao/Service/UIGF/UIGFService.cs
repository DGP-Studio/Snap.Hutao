// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUIGFService))]
internal sealed partial class UIGFService : IUIGFService
{
    private readonly IServiceProvider serviceProvider;

    public ValueTask ExportAsync(UIGFExportOptions exportOptions, CancellationToken token = default)
    {
        IUIGFExportService exportService = serviceProvider.GetRequiredKeyedService<IUIGFExportService>(UIGFVersion.UIGF40);
        return exportService.ExportAsync(exportOptions, token);
    }

    public ValueTask ImportAsync(UIGFImportOptions importOptions, CancellationToken token = default)
    {
        IUIGFImportService importService = serviceProvider.GetRequiredKeyedService<IUIGFImportService>(UIGFVersion.UIGF40);
        return importService.ImportAsync(importOptions, token);
    }
}