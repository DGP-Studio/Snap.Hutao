// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class ValueFileExtension
{
    public static async ValueTask<ValueResult<bool, T?>> DeserializeFromJsonNoThrowAsync<T>(this ValueFile file, JsonSerializerOptions options)
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
            SentrySdk.CaptureException(ex);
            return new(false, null);
        }
    }

    public static async ValueTask<bool> SerializeToJsonNoThrowAsync<T>(this ValueFile file, T obj, JsonSerializerOptions options)
    {
        try
        {
            using (FileStream stream = File.Create(file))
            {
                await JsonSerializer.SerializeAsync(stream, obj, options).ConfigureAwait(false);
            }

            return true;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return false;
        }
    }
}