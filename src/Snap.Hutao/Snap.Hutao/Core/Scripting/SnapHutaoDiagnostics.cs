// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity.Database;
using Windows.Storage;

namespace Snap.Hutao.Core.Scripting;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ISnapHutaoDiagnostics))]
internal sealed partial class SnapHutaoDiagnostics : ISnapHutaoDiagnostics
{
    private readonly IServiceProvider serviceProvider;

    public async ValueTask<string> GetPathFromApplicationUrlAsync(string url)
    {
        StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(url.ToUri());
        return file.Path;
    }

    public async ValueTask<int> ExecuteSqlAsync(string sql)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Database.ExecuteSqlRawAsync(sql).ConfigureAwait(false);
        }
    }
}