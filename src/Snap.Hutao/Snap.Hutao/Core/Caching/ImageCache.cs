﻿// Copyright (c) DGP Studio. All rights reserved.
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
[Injection(InjectAs.Singleton, typeof(IImageCache))]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(MaxConnectionsPerServer = 8)]
internal sealed class ImageCache : IImageCache, IImageCacheFilePathOperation
{
    private const string CacheFolderName = nameof(ImageCache);

    private static readonly FrozenDictionary<int, TimeSpan> RetryCountToDelay = new Dictionary<int, TimeSpan>()
    {
        [0] = TimeSpan.FromSeconds(4),
        [1] = TimeSpan.FromSeconds(16),
        [2] = TimeSpan.FromSeconds(64),
    }.ToFrozenDictionary();

    private readonly IHttpClientFactory httpClientFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ImageCache> logger;

    private readonly ConcurrentDictionary<string, Task> concurrentTasks = new();

    private string? baseFolder;
    private string? cacheFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageCache"/> class.
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public ImageCache(IServiceProvider serviceProvider)
    {
        logger = serviceProvider.GetRequiredService<ILogger<ImageCache>>();
        httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public void RemoveInvalid()
    {
        RemoveInternal(Directory.GetFiles(GetCacheFolder()).Where(file => IsFileInvalid(file, false)));
    }

    /// <inheritdoc/>
    public void Remove(Uri uriForCachedItem)
    {
        Remove(new ReadOnlySpan<Uri>(ref uriForCachedItem));
    }

    /// <inheritdoc/>
    public void Remove(in ReadOnlySpan<Uri> uriForCachedItems)
    {
        if (uriForCachedItems.Length <= 0)
        {
            return;
        }

        string folder = GetCacheFolder();
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

        RemoveInternal(filesToDelete);
    }

    /// <inheritdoc/>
    public async ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri)
    {
        string fileName = GetCacheFileName(uri);
        string filePath = Path.Combine(GetCacheFolder(), fileName);

        if (File.Exists(filePath) && new FileInfo(filePath).Length != 0)
        {
            return filePath;
        }

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

        return filePath;
    }

    /// <inheritdoc/>
    public ValueFile GetFileFromCategoryAndName(string category, string fileName)
    {
        Uri dummyUri = Web.HutaoEndpoints.StaticRaw(category, fileName).ToUri();
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
                logger.LogWarning(ex, "Remove Cache Image Failed:{File}", filePath);
            }
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task DownloadFileAsync(Uri uri, string baseFile)
    {
        logger.LogInformation("Begin downloading for {Uri}", uri);

        int retryCount = 0;
        HttpClient httpClient = httpClientFactory.CreateClient(nameof(ImageCache));
        while (retryCount < 3)
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

                switch (message.StatusCode)
                {
                    case HttpStatusCode.TooManyRequests:
                        {
                            retryCount++;
                            TimeSpan delay = message.Headers.RetryAfter?.Delta ?? RetryCountToDelay[retryCount];
                            logger.LogInformation("Retry {Uri} after {Delay}.", uri, delay);
                            await Task.Delay(delay).ConfigureAwait(false);
                            break;
                        }

                    default:
                        return;
                }
            }
        }
    }

    private string GetCacheFolder()
    {
        if (cacheFolder is not null)
        {
            return cacheFolder;
        }

        baseFolder ??= serviceProvider.GetRequiredService<RuntimeOptions>().LocalCache;
        DirectoryInfo info = Directory.CreateDirectory(Path.Combine(baseFolder, CacheFolderName));
        cacheFolder = info.FullName;

        return cacheFolder;
    }
}