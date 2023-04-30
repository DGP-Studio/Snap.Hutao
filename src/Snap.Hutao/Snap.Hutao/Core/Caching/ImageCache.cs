// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// Provides methods and tools to cache files in a folder
/// The class's name will become the cache folder's name
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(IImageCache))]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(MaxConnectionsPerServer = 8)]
internal sealed class ImageCache : IImageCache, IImageCacheFilePathOperation
{
    private const string CacheFolderName = nameof(ImageCache);

    private static readonly Dictionary<int, TimeSpan> RetryCountToDelay = new()
    {
        [0] = TimeSpan.FromSeconds(4),
        [1] = TimeSpan.FromSeconds(16),
        [2] = TimeSpan.FromSeconds(64),
        [3] = TimeSpan.FromSeconds(4),
        [4] = TimeSpan.FromSeconds(16),
        [5] = TimeSpan.FromSeconds(64),
    };

    private readonly ILogger logger;
    private readonly HttpClient httpClient;
    private readonly IServiceProvider serviceProvider;

    private readonly ConcurrentDictionary<string, Task> concurrentTasks = new();

    private string? baseFolder;
    private string? cacheFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageCache"/> class.
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="logger">日志器</param>
    /// <param name="httpClientFactory">http客户端工厂</param>
    public ImageCache(IServiceProvider serviceProvider)
    {
        logger = serviceProvider.GetRequiredService<ILogger<ImageCache>>();
        httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(ImageCache));

        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public void RemoveInvalid()
    {
        string folder = GetCacheFolder();
        string[] files = Directory.GetFiles(folder);

        List<string> filesToDelete = new();

        foreach (string file in files)
        {
            if (IsFileInvalid(file, false))
            {
                filesToDelete.Add(file);
            }
        }

        RemoveInternal(filesToDelete);
    }

    /// <inheritdoc/>
    public void Remove(Uri uriForCachedItem)
    {
        Remove(new ReadOnlySpan<Uri>(uriForCachedItem));
    }

    /// <inheritdoc/>
    public void Remove(in ReadOnlySpan<Uri> uriForCachedItems)
    {
        if (uriForCachedItems == null || uriForCachedItems.Length <= 0)
        {
            return;
        }

        string folder = GetCacheFolder();
        string[] files = Directory.GetFiles(folder);

        List<string> filesToDelete = new();

        foreach (Uri uri in uriForCachedItems)
        {
            string filePath = Path.Combine(folder, GetCacheFileName(uri));
            if (Array.IndexOf(files, filePath) >= 0)
            {
                filesToDelete.Add(filePath);
            }
        }

        RemoveInternal(filesToDelete);
    }

    /// <inheritdoc/>
    public async Task<string> GetFileFromCacheAsync(Uri uri)
    {
        string fileName = GetCacheFileName(uri);
        string filePath = Path.Combine(GetCacheFolder(), fileName);

        if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
        {
            TaskCompletionSource taskCompletionSource = new();
            try
            {
                if (concurrentTasks.TryAdd(fileName, taskCompletionSource.Task))
                {
                    await DownloadFileAsync(uri, filePath).ConfigureAwait(false);
                }
                else if (concurrentTasks.TryGetValue(fileName, out Task? task))
                {
                    await task.ConfigureAwait(false);
                }

                concurrentTasks.TryRemove(fileName, out _);
            }
            finally
            {
                taskCompletionSource.TrySetResult();
            }
        }

        return filePath;
    }

    /// <inheritdoc/>
    public string GetFilePathFromCategoryAndFileName(string category, string fileName)
    {
        Uri dummyUri = Web.HutaoEndpoints.StaticFile(category, fileName).ToUri();
        return Path.Combine(GetCacheFolder(), GetCacheFileName(dummyUri));
    }

    private static string GetCacheFileName(Uri uri)
    {
        string url = uri.ToString();
        byte[] chars = Encoding.UTF8.GetBytes(url);
        byte[] hash = SHA1.HashData(chars);
        return System.Convert.ToHexString(hash);
    }

    private static bool IsFileInvalid(string file, bool treatNullFileAsInvalid = true)
    {
        if (!File.Exists(file))
        {
            return treatNullFileAsInvalid;
        }

        FileInfo fileInfo = new(file);
        return fileInfo.Length == 0;
    }

    private void RemoveInternal(IEnumerable<string> filePaths)
    {
        foreach (string filePath in filePaths)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Remove Cache Image Failed:{file}", filePath);
            }
        }
    }

    private async Task DownloadFileAsync(Uri uri, string baseFile)
    {
        logger.LogInformation("Begin downloading for {uri}", uri);

        int retryCount = 0;
        while (retryCount < 6)
        {
            using (HttpResponseMessage message = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                if (message.IsSuccessStatusCode)
                {
                    using (Stream httpStream = await message.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        using (FileStream fileStream = File.Create(baseFile))
                        {
                            await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
                            return;
                        }
                    }
                }
                else if (message.StatusCode == HttpStatusCode.NotFound)
                {
                    // directly goto https://static.hut.ao
                    retryCount += 3;
                }
                else if (message.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    retryCount++;
                    TimeSpan delay = message.Headers.RetryAfter?.Delta ?? RetryCountToDelay[retryCount];
                    logger.LogInformation("Retry {uri} after {delay}.", uri, delay);
                    await Task.Delay(delay).ConfigureAwait(false);
                }
                else
                {
                    return;
                }
            }

            if (retryCount == 3)
            {
                uri = new UriBuilder(uri) { Host = Web.HutaoEndpoints.StaticHutao }.Uri;
            }
        }
    }

    private string GetCacheFolder()
    {
        if (cacheFolder == null)
        {
            baseFolder ??= serviceProvider.GetRequiredService<HutaoOptions>().LocalCache;
            DirectoryInfo info = Directory.CreateDirectory(Path.Combine(baseFolder, CacheFolderName));
            cacheFolder = info.FullName;
        }

        return cacheFolder!;
    }
}