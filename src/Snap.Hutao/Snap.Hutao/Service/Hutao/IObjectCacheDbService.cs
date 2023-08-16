// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Hutao;

internal interface IObjectCacheDbService
{
    ValueTask AddObjectCacheAsync<T>(string key, TimeSpan expire, T data)
        where T : class;

    ValueTask<T?> GetObjectOrDefaultAsync<T>(string key)
        where T : class;
}