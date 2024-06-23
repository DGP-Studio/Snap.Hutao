// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class ValueFileExtension
{
    public static async ValueTask<ValueResult<bool, T?>> DeserializeFromJsonAsync<T>(this ValueFile file, JsonSerializerOptions options)
        where T : class
    {
        try
        {
            using (FileStream stream = File.OpenRead(file))
            {
                T? t = await JsonSerializer.DeserializeAsync<T>(stream, options).ConfigureAwait(false);
                return new(true, t);
            }
        }
        catch (Exception ex)
        {
            _ = ex;
            return new(false, null);
        }
    }

    public static async ValueTask<bool> SerializeToJsonAsync<T>(this ValueFile file, T obj, JsonSerializerOptions options)
    {
        try
        {
            using (FileStream stream = File.Create(file))
            {
                await JsonSerializer.SerializeAsync(stream, obj, options).ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}