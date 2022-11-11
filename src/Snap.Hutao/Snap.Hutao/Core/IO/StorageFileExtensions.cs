// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.Storage;

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
    /// <returns>操作是否成功，反序列化后的内容</returns>
    public static async Task<ValueResult<bool, T?>> DeserializeFromJsonAsync<T>(this StorageFile file, JsonSerializerOptions options)
        where T : class
    {
        try
        {
            using (FileStream stream = File.OpenRead(file.Path))
            {
                T? t = await JsonSerializer.DeserializeAsync<T>(stream, options).ConfigureAwait(false);
                return new(true, t);
            }
        }
        catch (System.Exception ex)
        {
            _ = ex;
            return new(false, null);
        }
    }

    /// <summary>
    /// 将对象异步序列化入文件
    /// </summary>
    /// <typeparam name="T">对象的类型</typeparam>
    /// <param name="file">文件</param>
    /// <param name="obj">对象</param>
    /// <param name="options">序列化选项</param>
    /// <returns>操作是否成功</returns>
    public static async Task<bool> SerializeToJsonAsync<T>(this StorageFile file, T obj, JsonSerializerOptions options)
    {
        try
        {
            using (FileStream stream = File.Create(file.Path))
            {
                await JsonSerializer.SerializeAsync(stream, obj, options).ConfigureAwait(false);
            }

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}