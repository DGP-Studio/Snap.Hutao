// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.IO;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// Provides methods and tools to cache files in a folder
/// The class's name will become the cache folder's name
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IImageCache))]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(MaxConnectionsPerServer = 8)]
internal sealed partial class ImageCache : IImageCache, IImageCacheFilePathOperation
{
    private const string CacheFolderName = nameof(ImageCache);

    private readonly FrozenDictionary<int, TimeSpan> retryCountToDelay = new Dictionary<int, TimeSpan>()
    {
        [0] = TimeSpan.FromSeconds(4),
        [1] = TimeSpan.FromSeconds(16),
        [2] = TimeSpan.FromSeconds(64),
    }.ToFrozenDictionary();

    private readonly ConcurrentDictionary<string, Task> concurrentTasks = new();

    private readonly IHttpClientFactory httpClientFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ImageCache> logger;

    private string? baseFolder;
    private string? cacheFolder;

    private string CacheFolder
    {
        get => LazyInitializer.EnsureInitialized(ref cacheFolder, () =>
        {
            baseFolder ??= serviceProvider.GetRequiredService<RuntimeOptions>().LocalCache;
            DirectoryInfo info = Directory.CreateDirectory(Path.Combine(baseFolder, CacheFolderName));
            return info.FullName;
        });
    }

    /// <inheritdoc/>
    public void RemoveInvalid()
    {
        RemoveCore(Directory.GetFiles(CacheFolder).Where(file => IsFileInvalid(file, false)));
    }

    /// <inheritdoc/>
    public void Remove(Uri uriForCachedItem)
    {
        Remove([uriForCachedItem]);
    }

    /// <inheritdoc/>
    public void Remove(in ReadOnlySpan<Uri> uriForCachedItems)
    {
        if (uriForCachedItems.Length <= 0)
        {
            return;
        }

        string folder = CacheFolder;
        string[] files = Directory.GetFiles(folder);

        List<string> filesToDelete = [];
        foreach (ref readonly Uri uri in uriForCachedItems)
        {
            string filePath = Path.Combine(folder, GetCacheFileName(uri));
            if (files.Contains(filePath))
            {
                filesToDelete.Add(filePath);
            }
        }

        RemoveCore(filesToDelete);
    }

    /// <inheritdoc/>
    public async ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri)
    {
        string fileName = GetCacheFileName(uri);
        string filePath = Path.Combine(CacheFolder, fileName);

        if (!IsFileInvalid(filePath))
        {
            return filePath;
        }

        TaskCompletionSource taskCompletionSource = new();
        try
        {
            if (concurrentTasks.TryAdd(fileName, taskCompletionSource.Task))
            {
                logger.LogDebug("Begin downloading image file from '{Uri}' to '{File}'", uri, filePath);
                await DownloadFileAsync(uri, filePath).ConfigureAwait(false);
            }
            else if (concurrentTasks.TryGetValue(fileName, out Task? task))
            {
                logger.LogDebug("Waiting for a queued image download task to complete for '{Uri}'", uri);
                await task.ConfigureAwait(false);
            }

            concurrentTasks.TryRemove(fileName, out _);
        }
        finally
        {
            taskCompletionSource.TrySetResult();
        }

        return filePath;
    }

    /// <inheritdoc/>
    public ValueFile GetFileFromCategoryAndName(string category, string fileName)
    {
        Uri dummyUri = Web.HutaoEndpoints.StaticRaw(category, fileName).ToUri();
        return Path.Combine(CacheFolder, GetCacheFileName(dummyUri));
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

    private void RemoveCore(IEnumerable<string> filePaths)
    {
        foreach (string filePath in filePaths)
        {
            try
            {
                File.Delete(filePath);
                logger.LogInformation("Remove cached image succeed:{File}", filePath);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Remove cached image failed:{File}", filePath);
            }
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task DownloadFileAsync(Uri uri, string baseFile)
    {
        int retryCount = 0;
        HttpClient httpClient = httpClientFactory.CreateClient(nameof(ImageCache));
        while (retryCount < 3)
        {
            using (HttpResponseMessage message = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                if (message.RequestMessage is { RequestUri: { } target } && target != uri)
                {
                    logger.LogDebug("The Request '{Source}' has been redirected to '{Target}'", uri, target);
                }

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

                switch (message.StatusCode)
                {
                    case HttpStatusCode.TooManyRequests:
                        {
                            retryCount++;
                            TimeSpan delay = message.Headers.RetryAfter?.Delta ?? retryCountToDelay[retryCount];
                            logger.LogInformation("Retry download '{Uri}' after {Delay}.", uri, delay);
                            await Task.Delay(delay).ConfigureAwait(false);
                            break;
                        }

                    default:
                        return;
                }
            }
        }
    }
}