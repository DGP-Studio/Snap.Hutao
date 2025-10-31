// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

[Service(ServiceLifetime.Singleton, typeof(IUIGFService))]
internal sealed partial class UIGFService : IUIGFService
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial UIGFService(IServiceProvider serviceProvider);

    public ValueTask ExportAsync(UIGFExportOptions exportOptions, CancellationToken token = default)
    {
        IUIGFExportService exportService = serviceProvider.GetRequiredKeyedService<IUIGFExportService>(UIGFVersion.UIGF41);
        return exportService.ExportAsync(exportOptions, token);
    }

    public ValueTask ImportAsync(UIGFImportOptions importOptions, CancellationToken token = default)
    {
        UIGFVersion version = importOptions.UIGF.Info.Version switch
        {
            "v4.0" => UIGFVersion.UIGF40,
            "v4.1" => UIGFVersion.UIGF41,
            _ => UIGFVersion.None,
        };

        IUIGFImportService importService = serviceProvider.GetRequiredKeyedService<IUIGFImportService>(version);
        return importService.ImportAsync(importOptions, token);
    }
}