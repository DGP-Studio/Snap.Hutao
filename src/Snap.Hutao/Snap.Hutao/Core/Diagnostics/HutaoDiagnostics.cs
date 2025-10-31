// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity.Database;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;

namespace Snap.Hutao.Core.Diagnostics;

[Service(ServiceLifetime.Singleton, typeof(IHutaoDiagnostics))]
internal sealed partial class HutaoDiagnostics : IHutaoDiagnostics
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial HutaoDiagnostics(IServiceProvider serviceProvider);

    public ApplicationDataContainer LocalSettings { get => ApplicationData.Current.LocalSettings; }

    public async ValueTask<int> ExecuteSqlAsync(string sql)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Database.ExecuteSqlRawAsync(sql).ConfigureAwait(false);
        }
    }

    public ApplicationDataCompositeValue MakeApplicationDataCompositeValue(params string[] values)
    {
        ApplicationDataCompositeValue compositeValue = [];
        foreach (string value in values)
        {
            compositeValue.Add($"{CryptographicOperations.HashData(HashAlgorithmName.MD5, Encoding.UTF8.GetBytes(value)):X}", value);
        }

        return compositeValue;
    }
}