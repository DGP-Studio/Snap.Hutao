// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Hutao;

[Service(ServiceLifetime.Singleton, typeof(IObjectCacheRepository))]
internal sealed partial class ObjectCacheRepository : IObjectCacheRepository
{
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial ObjectCacheRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public async ValueTask AddObjectCacheAsync<T>(string key, TimeSpan expire, T data)
        where T : class
    {
        await taskContext.SwitchToBackgroundAsync();
        this.Add(new()
        {
            Key = key,
            ExpireTime = DateTimeOffset.Now.Add(expire),
            Value = JsonSerializer.Serialize(data, jsonSerializerOptions),
        });
    }

    public async ValueTask<T?> GetObjectOrDefaultAsync<T>(string key)
        where T : class
    {
        await taskContext.SwitchToBackgroundAsync();
        if (this.SingleOrDefault(e => e.Key == key) is { } entry)
        {
            if (!entry.IsExpired)
            {
                ArgumentNullException.ThrowIfNull(entry.Value);
                return JsonSerializer.Deserialize<T>(entry.Value, jsonSerializerOptions);
            }

            this.Delete(entry);
        }

        return default;
    }
}