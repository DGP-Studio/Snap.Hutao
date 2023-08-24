// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Hutao;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IObjectCacheDbService))]
internal sealed partial class ObjectCacheDbService : IObjectCacheDbService
{
    private readonly IServiceProvider serviceProvider;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public async ValueTask AddObjectCacheAsync<T>(string key, TimeSpan expire, T data)
        where T : class
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await appDbContext.ObjectCache.AddAndSaveAsync(new()
            {
                Key = key,
                ExpireTime = DateTimeOffset.Now.Add(expire),
                Value = JsonSerializer.Serialize(data, jsonSerializerOptions),
            }).ConfigureAwait(false);
        }
    }

    public async ValueTask<T?> GetObjectOrDefaultAsync<T>(string key)
        where T : class
    {
        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (await appDbContext.ObjectCache.SingleOrDefaultAsync(e => e.Key == key).ConfigureAwait(false) is { } entry)
                {
                    if (!entry.IsExpired)
                    {
                        ArgumentNullException.ThrowIfNull(entry.Value);
                        T? value = JsonSerializer.Deserialize<T>(entry.Value, jsonSerializerOptions);
                        return value;
                    }

                    await appDbContext.ObjectCache.RemoveAndSaveAsync(entry).ConfigureAwait(false);
                }
            }
        }
        catch (DbUpdateException ex)
        {
            ThrowHelper.DatabaseCorrupted($"无法存储 Key:{key} 对应的值到数据库缓存", ex);
        }

        return default!;
    }
}