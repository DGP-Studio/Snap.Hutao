// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Context.FileSystem.Location;
using Windows.Storage;

namespace Snap.Hutao.Context.FileSystem;

/// <summary>
/// 缓存目录上下文
/// </summary>
[Injection(InjectAs.Transient)]
internal class CacheContext : FileSystemContext
{
    /// <summary>
    /// 构造一个新的缓存目录上下文
    /// </summary>
    /// <param name="cache">缓存位置</param>
    public CacheContext(Cache cache)
        : base(cache)
    {
    }

    /// <summary>
    /// 获取缓存文件夹
    /// </summary>
    /// <param name="folderName">文件夹名称</param>
    /// <param name="token">取消令牌</param>
    /// <returns>缓存文件夹</returns>
    public static Task<StorageFolder> GetFolderAsync(string folderName, CancellationToken token)
    {
        StorageFolder tempstate = ApplicationData.Current.TemporaryFolder;
        return tempstate.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists).AsTask(token);
    }

    /// <summary>
    /// 获取缓存文件的名称
    /// </summary>
    /// <param name="uri">uri</param>
    /// <returns>缓存文件的名称</returns>
    public static string GetCacheFileName(Uri uri)
    {
        return CreateHash64(uri.ToString()).ToString();
    }

    /// <summary>
    /// 获取缓存文件的名称
    /// </summary>
    /// <param name="url">url</param>
    /// <returns>缓存文件的名称</returns>
    public static string GetCacheFileName(string url)
    {
        return CreateHash64(url).ToString();
    }

    private static ulong CreateHash64(string str)
    {
        byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(str);

        ulong value = (ulong)utf8.Length;
        for (int n = 0; n < utf8.Length; n++)
        {
            value += (ulong)utf8[n] << ((n * 5) % 56);
        }

        return value;
    }
}