// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.UI;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Win32.System.WinRT;
using System.IO;
using System.Security.Cryptography;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using WinRT;
using ThemeFile = (Microsoft.UI.Xaml.ElementTheme, Snap.Hutao.Core.IO.ValueFile);

namespace Snap.Hutao.Core.Caching;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IImageCache))]
internal sealed partial class ImageCache : IImageCache, IImageCacheFilePathOperation
{
    private readonly AsyncKeyedLock<ThemeFile> themeFileLocks = new();
    private readonly AsyncKeyedLock<string> downloadLocks = new();

    private readonly IImageCacheDownloadOperation downloadOperation;
    private readonly ILogger<ImageCache> logger;

    private string? cacheFolder;

    private string CacheFolder
    {
        get => LazyInitializer.EnsureInitialized(ref cacheFolder, () =>
        {
            string folder = HutaoRuntime.GetLocalCacheImageCacheFolder();
            Directory.CreateDirectory(Path.Combine(folder, "Light"));
            Directory.CreateDirectory(Path.Combine(folder, "Dark"));
            return folder;
        });
    }

    public void Remove(Uri uriForCachedItem)
    {
        string filePath = Path.Combine(CacheFolder, GetCacheFileName(uriForCachedItem));
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

    public ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri)
    {
        return GetFileFromCacheAsync(uri, ElementTheme.Default);
    }

    public async ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri, ElementTheme theme)
    {
        string fileName = GetCacheFileName(uri);
        string defaultFilePath = Path.Combine(CacheFolder, fileName);
        string themeOrDefaultFilePath = theme is ElementTheme.Dark or ElementTheme.Light
            ? Path.Combine(CacheFolder, $"{theme}", fileName)
            : defaultFilePath;

        if (!IsFileInvalid(themeOrDefaultFilePath))
        {
            return themeOrDefaultFilePath;
        }

        using (await themeFileLocks.LockAsync((theme, fileName)).ConfigureAwait(false))
        {
            // If the file already exists, we don't need to download it again
            if (!IsFileInvalid(defaultFilePath))
            {
                await TryConvertToMonoChromeAndSaveFileAsync(defaultFilePath, themeOrDefaultFilePath, theme).ConfigureAwait(false);
                return themeOrDefaultFilePath;
            }

            using (await downloadLocks.LockAsync(fileName).ConfigureAwait(false))
            {
                // File may be downloaded by another thread
                if (!IsFileInvalid(defaultFilePath))
                {
                    await TryConvertToMonoChromeAndSaveFileAsync(defaultFilePath, themeOrDefaultFilePath, theme).ConfigureAwait(false);
                    return themeOrDefaultFilePath;
                }

                logger.LogColorizedInformation("Begin to download file from '{Uri}' to '{File}'", (uri, ConsoleColor.Cyan), (defaultFilePath, ConsoleColor.Cyan));
                await downloadOperation.DownloadFileAsync(uri, defaultFilePath).ConfigureAwait(false);
                return themeOrDefaultFilePath;
            }
        }
    }

    public ValueFile GetFileFromCategoryAndName(string category, string fileName)
    {
        return Path.Combine(CacheFolder, GetCacheFileName(StaticResourcesEndpoints.StaticRaw(category, fileName)));
    }

    private static string GetCacheFileName(Uri uri)
    {
        return GetCacheFileName(uri.ToString());
    }

    private static string GetCacheFileName(string url)
    {
        return Hash.ToHexString(HashAlgorithmName.SHA1, url);
    }

    private static bool IsFileInvalid(string file, bool treatNullFileAsInvalid = true)
    {
        if (!File.Exists(file))
        {
            return treatNullFileAsInvalid;
        }

        return new FileInfo(file).Length == 0;
    }

    private static async ValueTask TryConvertToMonoChromeAndSaveFileAsync(string sourceFile, string themeFile, ElementTheme theme)
    {
        if (theme is ElementTheme.Default || string.Equals(sourceFile, themeFile, StringComparison.OrdinalIgnoreCase))
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
                        byte value = theme is ElementTheme.Light ? (byte)0x00 : (byte)0xFF;
                        SyncConvertToMonoChrome(reference.As<IMemoryBufferByteAccess>(), value);
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

        static void SyncConvertToMonoChrome(IMemoryBufferByteAccess byteAccess, byte background)
        {
            byteAccess.GetBuffer(out Span<Rgba32> span);
            foreach (ref Rgba32 pixel in span)
            {
                pixel.A = (byte)pixel.Luminance255;
                pixel.R = pixel.G = pixel.B = background;
            }
        }
    }
}