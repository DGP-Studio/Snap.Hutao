// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 文件拓展
/// </summary>
public static class StorageFileExtensions
{
    /// <summary>
    /// 异步反序列化文件中的内容
    /// </summary>
    /// <typeparam name="T">内容的类型</typeparam>
    /// <param name="file">文件</param>
    /// <param name="options">序列化选项</param>
    /// <param name="onException">错误时调用</param>
    /// <returns>反序列化后的内容</returns>
    public static async Task<T?> DeserializeJsonAsync<T>(this StorageFile file, JsonSerializerOptions options, Action<System.Exception>? onException = null)
        where T : class
    {
        T? t = null;
        try
        {
            using (IRandomAccessStreamWithContentType fileSream = await file.OpenReadAsync())
            {
                using (Stream stream = fileSream.AsStream())
                {
                    t = await JsonSerializer.DeserializeAsync<T>(stream, options).ConfigureAwait(false);
                }
            }
        }
        catch (System.Exception ex)
        {
            onException?.Invoke(ex);
        }

        return t;
    }
}