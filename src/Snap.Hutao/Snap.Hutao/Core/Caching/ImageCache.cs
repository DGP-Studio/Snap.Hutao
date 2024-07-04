// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.UI;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Win32.System.WinRT;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using WinRT;

namespace Snap.Hutao.Core.Caching;

[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IImageCache))]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(MaxConnectionsPerServer = 8)]
internal sealed partial class ImageCache : IImageCache, IImageCacheFilePathOperation
{
    private const string CacheFailedDownloadTasksName = $"{nameof(ImageCache)}.FailedDownloadTasks";

    private static readonly FrozenDictionary<int, TimeSpan> DelayFromRetryCount = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(0, TimeSpan.FromSeconds(4)),
        KeyValuePair.Create(1, TimeSpan.FromSeconds(16)),
        KeyValuePair.Create(2, TimeSpan.FromSeconds(64)),
    ]);

    private readonly ConcurrentDictionary<ElementThemeValueFile, Task> themefileTasks = [];
    private readonly ConcurrentDictionary<string, Task> downloadTasks = [];

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ImageCache> logger;
    private readonly IMemoryCache memoryCache;

    private string? cacheFolder;

    private string CacheFolder
    {
        get => LazyInitializer.EnsureInitialized(ref cacheFolder, () =>
        {
            return serviceProvider.GetRequiredService<RuntimeOptions>().GetLocalCacheImageCacheFolder();
        });
    }

    public void Remove(Uri uriForCachedItem)
    {
        Remove([uriForCachedItem]);
    }

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

    public ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri)
    {
        return GetFileFromCacheAsync(uri, ElementTheme.Default);
    }

    public async ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri, ElementTheme theme)
    {
        string fileName = GetCacheFileName(uri);

        using (ScopedTaskCompletionSource themeFileScope = new())
        {
            ElementThemeValueFile key = new(fileName, theme);
            if (themefileTasks.TryAdd(key, themeFileScope.Task))
            {
                try
                {
                    string defaultFilePath = Path.Combine(CacheFolder, fileName);
                    string themeOrDefaultFilePath = theme is ElementTheme.Dark or ElementTheme.Light ? Path.Combine(CacheFolder, $"{theme}", fileName) : defaultFilePath;

                    if (!IsFileInvalid(themeOrDefaultFilePath))
                    {
                        return themeOrDefaultFilePath;
                    }

                    if (!IsFileInvalid(defaultFilePath))
                    {
                        await ConvertAndSaveFileToMonoChromeAsync(defaultFilePath, themeOrDefaultFilePath, theme).ConfigureAwait(false);
                        return themeOrDefaultFilePath;
                    }

                    using (ScopedTaskCompletionSource downloadScope = new())
                    {
                        if (downloadTasks.TryAdd(fileName, downloadScope.Task))
                        {
                            try
                            {
                                logger.LogColorizedInformation("Begin to download file from '{Uri}' to '{File}'", (uri, ConsoleColor.Cyan), (defaultFilePath, ConsoleColor.Cyan));
                                await DownloadFileAsync(uri, defaultFilePath).ConfigureAwait(false);
                            }
                            finally
                            {
                                downloadTasks.TryRemove(fileName, out _);
                            }
                        }
                        else if (downloadTasks.TryGetValue(fileName, out Task? task))
                        {
                            logger.LogDebug("Waiting for a queued image download task to complete for '{Uri}'", (uri, ConsoleColor.Cyan));
                            await task.ConfigureAwait(false);
                        }
                    }

                    if (!IsFileInvalid(defaultFilePath))
                    {
                        await ConvertAndSaveFileToMonoChromeAsync(defaultFilePath, themeOrDefaultFilePath, theme).ConfigureAwait(false);
                        return themeOrDefaultFilePath;
                    }

                    return themeOrDefaultFilePath;
                }
                finally
                {
                    themefileTasks.TryRemove(key, out _);
                }
            }
            else if (themefileTasks.TryGetValue(key, out Task? task))
            {
                await task.ConfigureAwait(false);
                return key.File;
            }
        }

        throw HutaoException.NotSupported("The task should not be null.");
    }

    /// <inheritdoc/>
    public ValueFile GetFileFromCategoryAndName(string category, string fileName)
    {
        Uri dummyUri = HutaoEndpoints.StaticRaw(category, fileName).ToUri();
        return Path.Combine(CacheFolder, GetCacheFileName(dummyUri));
    }

    private static string GetCacheFileName(Uri uri)
    {
        return Hash.SHA1HexString(uri.ToString());
    }

    private static bool IsFileInvalid(string file, bool treatNullFileAsInvalid = true)
    {
        if (!File.Exists(file))
        {
            return treatNullFileAsInvalid;
        }

        return new FileInfo(file).Length == 0;
    }

    private static async ValueTask ConvertAndSaveFileToMonoChromeAsync(string sourceFile, string themeFile, ElementTheme theme)
    {
        if (string.Equals(sourceFile, themeFile, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        using (FileStream sourceStream = File.OpenRead(sourceFile))
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream.AsRandomAccessStream());

            // Always premultiplied to prevent some channels have a non-zero value when the alpha channel is zero
            using (SoftwareBitmap sourceBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied))
            {
                using (BitmapBuffer sourceBuffer = sourceBitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
                {
                    using (IMemoryBufferReference reference = sourceBuffer.CreateReference())
                    {
                        IMemoryBufferByteAccess byteAccess = reference.As<IMemoryBufferByteAccess>();
                        byte value = theme is ElementTheme.Light ? (byte)0x00 : (byte)0xFF;
                        ConvertToMonoChrome(byteAccess, value);
                    }
                }

                using (FileStream themeStream = File.Create(themeFile))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, themeStream.AsRandomAccessStream());
                    encoder.SetSoftwareBitmap(sourceBitmap);
                    await encoder.FlushAsync();
                }
            }
        }

        static void ConvertToMonoChrome(IMemoryBufferByteAccess byteAccess, byte background)
        {
            byteAccess.GetBuffer(out Span<Rgba32> span);
            foreach (ref Rgba32 pixel in span)
            {
                pixel.A = (byte)(pixel.Luminance * 255);
                pixel.R = pixel.G = pixel.B = background;
            }
        }
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
        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(ImageCache)))
        {
            int retryCount = 0;

            HttpRequestMessageBuilder requestMessageBuilder = httpRequestMessageBuilderFactory
                .Create()
                .SetRequestUri(uri)
                .SetStaticResourceControlHeadersIf(uri.Host.Contains("api.snapgenshin.com", StringComparison.OrdinalIgnoreCase)) // These headers are only available for our own api
                .Get();

            while (retryCount < 3)
            {
                requestMessageBuilder.Resurrect();

                using (HttpRequestMessage requestMessage = requestMessageBuilder.HttpRequestMessage)
                {
                    using (HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        // Redirect detection
                        if (responseMessage.RequestMessage is { RequestUri: { } target } && target != uri)
                        {
                            logger.LogDebug("The Request '{Source}' has been redirected to '{Target}'", uri, target);
                        }

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            if (responseMessage.Content.Headers.ContentType?.MediaType is "application/json")
                            {
                                DebugTrackFailedUri(uri);
                                string raw = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                                logger.LogColorizedCritical("Failed to download '{Uri}' with unexpected body '{Raw}'", (uri, ConsoleColor.Red), (raw, ConsoleColor.DarkYellow));
                                return;
                            }

                            using (Stream httpStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                using (FileStream fileStream = File.Create(baseFile))
                                {
                                    await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                    return;
                                }
                            }
                        }

                        switch (responseMessage.StatusCode)
                        {
                            case HttpStatusCode.TooManyRequests:
                                {
                                    retryCount++;
                                    TimeSpan delay = responseMessage.Headers.RetryAfter?.Delta ?? DelayFromRetryCount[retryCount];
                                    logger.LogInformation("Retry download '{Uri}' after {Delay}.", uri, delay);
                                    await Task.Delay(delay).ConfigureAwait(false);
                                    break;
                                }

                            default:
                                DebugTrackFailedUri(uri);
                                logger.LogColorizedCritical("Failed to download '{Uri}' with status code '{StatusCode}'", (uri, ConsoleColor.Red), (responseMessage.StatusCode, ConsoleColor.DarkYellow));
                                return;
                        }
                    }
                }
            }
        }
    }

    [Conditional("DEBUG")]
    private void DebugTrackFailedUri(Uri uri)
    {
        HashSet<string>? set = memoryCache.GetOrCreate(CacheFailedDownloadTasksName, entry => new HashSet<string>());
        set?.Add(uri.ToString());
    }

    private readonly struct ElementThemeValueFile
    {
        public readonly ValueFile File;
        public readonly ElementTheme Theme;

        public ElementThemeValueFile(ValueFile file, ElementTheme theme)
        {
            File = file;
            Theme = theme;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(File, Theme);
        }
    }
}