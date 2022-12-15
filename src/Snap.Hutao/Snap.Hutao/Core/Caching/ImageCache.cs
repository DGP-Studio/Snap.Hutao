// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.Logging;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// Provides methods and tools to cache files in a folder
/// The class's name will become the cache folder's name
/// </summary>
[Injection(InjectAs.Singleton, typeof(IImageCache))]
[HttpClient(HttpClientConfigration.Default)]
[PrimaryHttpMessageHandler(MaxConnectionsPerServer = 16)]
[SuppressMessage("", "CA1001")]
public class ImageCache : IImageCache
{
    private const string DateAccessedProperty = "System.DateAccessed";

    private static readonly ImmutableDictionary<int, TimeSpan> RetryCountToDelay = new Dictionary<int, TimeSpan>()
    {
        [0] = TimeSpan.FromSeconds(4),
        [1] = TimeSpan.FromSeconds(16),
        [2] = TimeSpan.FromSeconds(64),
        [3] = TimeSpan.FromSeconds(4),
        [4] = TimeSpan.FromSeconds(16),
        [5] = TimeSpan.FromSeconds(64),
    }.ToImmutableDictionary();

    private readonly List<string> extendedPropertyNames = new() { DateAccessedProperty };

    private readonly SemaphoreSlim cacheFolderSemaphore = new(1);
    private readonly ILogger logger;

    // violate di rule
    private readonly HttpClient httpClient;

    private StorageFolder? baseFolder;
    private string? cacheFolderName;
    private StorageFolder? cacheFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageCache"/> class.
    /// </summary>
    /// <param name="logger">日志器</param>
    /// <param name="httpClientFactory">http客户端工厂</param>
    public ImageCache(ILogger<ImageCache> logger, IHttpClientFactory httpClientFactory)
    {
        this.logger = logger;
        httpClient = httpClientFactory.CreateClient(nameof(ImageCache));

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

        if (item == null || (await item.GetBasicPropertiesAsync()).Size == 0)
        {
            StorageFile baseFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
            await DownloadFileAsync(uri, baseFile).ConfigureAwait(false);
            item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);
        }

        return Must.NotNull((item as StorageFile)!);
    }

    private static string GetCacheFileName(Uri uri)
    {
        string url = uri.ToString();
        byte[] chars = Encoding.UTF8.GetBytes(url);
        byte[] hash = SHA1.HashData(chars);
        return System.Convert.ToHexString(hash);
    }

    /// <summary>
    /// Override-able method that checks whether file is valid or not.
    /// </summary>
    /// <param name="file">storage file</param>
    /// <param name="duration">cache duration</param>
    /// <param name="treatNullFileAsOutOfDate">option to mark uninitialized file as expired</param>
    /// <returns>bool indicate whether file has expired or not</returns>
    private async Task<bool> IsFileOutOfDateAsync(StorageFile file, TimeSpan duration, bool treatNullFileAsOutOfDate = true)
    {
        if (file == null)
        {
            return treatNullFileAsOutOfDate;
        }

        // Get extended properties.
        IDictionary<string, object> extraProperties = await file.Properties
            .RetrievePropertiesAsync(extendedPropertyNames)
            .AsTask()
            .ConfigureAwait(false);

        // Get date-accessed property.
        object? propValue = extraProperties[DateAccessedProperty];

        if (propValue != null)
        {
            DateTimeOffset? lastAccess = propValue as DateTimeOffset?;

            if (lastAccess.HasValue)
            {
                return DateTime.Now.Subtract(lastAccess.Value.DateTime) > duration;
            }
        }

        BasicProperties properties = await file
            .GetBasicPropertiesAsync()
            .AsTask()
            .ConfigureAwait(false);

        return properties.Size == 0 || DateTime.Now.Subtract(properties.DateModified.DateTime) > duration;
    }

    private async Task DownloadFileAsync(Uri uri, StorageFile baseFile)
    {
        logger.LogInformation(EventIds.FileCaching, "Begin downloading for {uri}", uri);

        int retryCount = 0;
        while (retryCount < 6)
        {
            using (HttpResponseMessage message = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                if (message.IsSuccessStatusCode)
                {
                    using (Stream httpStream = await message.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (FileStream fileStream = File.Create(baseFile.Path))
                        {
                            await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
                            return;
                        }
                    }
                }
                else if (message.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    retryCount++;
                    TimeSpan delay = message.Headers.RetryAfter?.Delta ?? RetryCountToDelay[retryCount];
                    logger.LogInformation("Retry after {delay}.", delay);
                    await Task.Delay(delay).ConfigureAwait(false);
                }
                else
                {
                    return;
                }
            }

            if (retryCount == 3)
            {
                uri = new UriBuilder(uri) { Host = "static.hut.ao", }.Uri;
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
                logger.LogInformation(EventIds.CacheRemoveFile, "Removing file {file}", file.Path);
                await file.DeleteAsync().AsTask().ConfigureAwait(false);
            }
            catch
            {
                logger.LogError(EventIds.CacheException, "Failed to delete file: {file}", file.Path);
            }
        }
    }
}