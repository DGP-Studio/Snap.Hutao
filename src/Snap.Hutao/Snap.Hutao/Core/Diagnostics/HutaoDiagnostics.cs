// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity.Database;
using Windows.Storage;

namespace Snap.Hutao.Core.Diagnostics;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IHutaoDiagnostics))]
internal sealed partial class HutaoDiagnostics : IHutaoDiagnostics
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// <see cref="Core.Setting.LocalSetting"/>
    /// </summary>
    public ApplicationDataContainer LocalSetting { get => ApplicationData.Current.LocalSettings; }

    public async ValueTask<int> ExecuteSqlAsync(string sql)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Database.ExecuteSqlRawAsync(sql).ConfigureAwait(false);
        }
    }
}