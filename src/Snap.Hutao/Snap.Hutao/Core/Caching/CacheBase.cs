// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Snap.Hutao.Core.Logging;
using Snap.Hutao.Extension;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// Provides methods and tools to cache files in a folder
/// 经过简化
/// </summary>
/// <typeparam name="T">Generic type as supplied by consumer of the class</typeparam>
public abstract class CacheBase<T>
    where T : class
{
    private readonly SemaphoreSlim cacheFolderSemaphore = new(1);
    private readonly ILogger logger;
    private readonly HttpClient httpClient;

    private StorageFolder? baseFolder;
    private string? cacheFolderName;
    private StorageFolder? cacheFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheBase{T}"/> class.
    /// </summary>
    /// <param name="logger">日志器</param>
    /// <param name="httpClient">http客户端</param>
    protected CacheBase(ILogger logger, HttpClient httpClient)
    {
        this.logger = logger;
        this.httpClient = httpClient;

        CacheDuration = TimeSpan.FromDays(30);
        RetryCount = 3;
    }

    /// <summary>
    /// Gets or sets the life duration of every cache entry.
    /// </summary>
    public TimeSpan CacheDuration { get; }

    /// <summary>
    /// Gets or sets the number of retries trying to ensure the file is cached.
    /// </summary>
    public uint RetryCount { get; }

    /// <summary>
    /// Clears all files in the cache
    /// </summary>
    /// <returns>awaitable task</returns>
    public async Task ClearAsync()
    {
        StorageFolder folder = await GetCacheFolderAsync().ConfigureAwait(false);
        IReadOnlyList<StorageFile> files = await folder.GetFilesAsync().AsTask().ConfigureAwait(false);

        await RemoveAsync(files).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes cached files that have expired
    /// </summary>
    /// <param name="duration">Optional timespan to compute whether file has expired or not. If no value is supplied, <see cref="CacheDuration"/> is used.</param>
    /// <returns>awaitable task</returns>
    public async Task RemoveExpiredAsync(TimeSpan? duration = null)
    {
        TimeSpan expiryDuration = duration ?? CacheDuration;

        StorageFolder folder = await GetCacheFolderAsync().ConfigureAwait(false);
        IReadOnlyList<StorageFile> files = await folder.GetFilesAsync().AsTask().ConfigureAwait(false);

        List<StorageFile> filesToDelete = new();

        foreach (StorageFile file in files)
        {
            if (file == null)
            {
                continue;
            }

            if (await IsFileOutOfDateAsync(file, expiryDuration, false).ConfigureAwait(false))
            {
                filesToDelete.Add(file);
            }
        }

        await RemoveAsync(filesToDelete).ConfigureAwait(false);
    }

    /// <summary>
    /// Removed items based on uri list passed
    /// </summary>
    /// <param name="uriForCachedItems">Enumerable uri list</param>
    /// <returns>awaitable Task</returns>
    public async Task RemoveAsync(IEnumerable<Uri> uriForCachedItems)
    {
        if (uriForCachedItems == null || !uriForCachedItems.Any())
        {
            return;
        }

        StorageFolder folder = await GetCacheFolderAsync().ConfigureAwait(false);
        IReadOnlyList<StorageFile> files = await folder.GetFilesAsync().AsTask().ConfigureAwait(false);

        List<StorageFile> filesToDelete = new();

        Dictionary<string, StorageFile> cachedFiles = files.ToDictionary(file => file.Name);

        foreach (Uri uri in uriForCachedItems)
        {
            string fileName = GetCacheFileName(uri);
            if (cachedFiles.TryGetValue(fileName, out StorageFile? file))
            {
                filesToDelete.Add(file);
            }
        }

        await RemoveAsync(filesToDelete).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the StorageFile containing cached item for given Uri
    /// </summary>
    /// <param name="uri">Uri of the item.</param>
    /// <returns>a StorageFile</returns>
    public async Task<StorageFile> GetFileFromCacheAsync(Uri uri)
    {
        StorageFolder folder = await GetCacheFolderAsync().ConfigureAwait(false);

        string fileName = GetCacheFileName(uri);

        IStorageItem? item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);

        if (item == null)
        {
            StorageFile baseFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
            await DownloadFileAsync(uri, baseFile).ConfigureAwait(false);
            item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);
        }

        return Must.NotNull((item as StorageFile)!);
    }

    /// <summary>
    /// Override-able method that checks whether file is valid or not.
    /// </summary>
    /// <param name="file">storage file</param>
    /// <param name="duration">cache duration</param>
    /// <param name="treatNullFileAsOutOfDate">option to mark uninitialized file as expired</param>
    /// <returns>bool indicate whether file has expired or not</returns>
    protected virtual async Task<bool> IsFileOutOfDateAsync(StorageFile file, TimeSpan duration, bool treatNullFileAsOutOfDate = true)
    {
        if (file == null)
        {
            return treatNullFileAsOutOfDate;
        }

        BasicProperties? properties = await file.GetBasicPropertiesAsync().AsTask().ConfigureAwait(false);

        return properties.Size == 0 || DateTime.Now.Subtract(properties.DateModified.DateTime) > duration;
    }

    private static string GetCacheFileName(Uri uri)
    {
        return CreateHash64(uri.ToString()).ToString();
    }

    private static ulong CreateHash64(string str)
    {
        byte[] utf8 = Encoding.UTF8.GetBytes(str);

        ulong value = (ulong)utf8.Length;
        for (int n = 0; n < utf8.Length; n++)
        {
            value += (ulong)utf8[n] << ((n * 5) % 56);
        }

        return value;
    }

    private async Task DownloadFileAsync(Uri uri, StorageFile baseFile)
    {
        logger.LogInformation(EventIds.FileCaching, "Begin downloading for {uri}", uri);

        using (Stream httpStream = await httpClient.GetStreamAsync(uri))
        {
            using (Stream fileStream = await baseFile.OpenStreamForWriteAsync())
            {
                await httpStream.CopyToAsync(fileStream);

                // Call this before dispose fileStream.
                await fileStream.FlushAsync();
            }
        }
    }

    /// <summary>
    /// Initializes with default values if user has not initialized explicitly
    /// </summary>
    /// <returns>awaitable task</returns>
    private async Task InitializeInternalAsync()
    {
        if (cacheFolder != null)
        {
            return;
        }

        using (await cacheFolderSemaphore.EnterAsync().ConfigureAwait(false))
        {
            baseFolder ??= ApplicationData.Current.TemporaryFolder;

            if (string.IsNullOrWhiteSpace(cacheFolderName))
            {
                cacheFolderName = GetType().Name;
            }

            cacheFolder = await baseFolder
                .CreateFolderAsync(cacheFolderName, CreationCollisionOption.OpenIfExists)
                .AsTask()
                .ConfigureAwait(false);
        }
    }

    private async Task<StorageFolder> GetCacheFolderAsync()
    {
        if (cacheFolder == null)
        {
            await InitializeInternalAsync().ConfigureAwait(false);
        }

        return Must.NotNull(cacheFolder!);
    }

    private async Task RemoveAsync(IEnumerable<StorageFile> files)
    {
        foreach (StorageFile file in files)
        {
            try
            {
                logger.LogInformation(EventIds.CacheRemoveFile, "Removing file {file}", file);
                await file.DeleteAsync().AsTask().ConfigureAwait(false);
            }
            catch
            {
                logger.LogError(EventIds.CacheException, "Failed to delete file: {file}", file.Path);
            }
        }
    }
}