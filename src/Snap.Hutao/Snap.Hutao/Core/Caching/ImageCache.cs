// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.Process;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Win32;
using System.Collections.Frozen;
using System.Diagnostics;
using System.IO;
using ThemeFile = (Microsoft.UI.Xaml.ElementTheme, Snap.Hutao.Core.IO.ValueFile);

namespace Snap.Hutao.Core.Caching;

[Service(ServiceLifetime.Singleton, typeof(IImageCache))]
internal sealed partial class ImageCache : IImageCache
{
    private static readonly FrozenSet<string> SupportedSchemes =
    [
        Uri.UriSchemeHttp,
        Uri.UriSchemeHttps,
    ];

    private readonly AsyncKeyedLock<ThemeFile> themeFileLocks = new();
    private readonly AsyncKeyedLock<string> downloadLocks = new();

    private readonly IImageCacheDownloadOperation downloadOperation;

    [GeneratedConstructor]
    public partial ImageCache(IServiceProvider serviceProvider);

    private string CacheFolder
    {
        get => LazyInitializer.EnsureInitialized(ref field, static () =>
        {
            try
            {
                string folder = HutaoRuntime.GetLocalCacheImageCacheDirectory();
                Directory.CreateDirectory(Path.Combine(folder, "Light"));
                Directory.CreateDirectory(Path.Combine(folder, "Dark"));
                return folder;
            }
            catch (Exception ex)
            {
                // 0x80070570 ERROR_FILE_CORRUPT
                HutaoNative.Instance.ShowErrorMessage(ex.Message, ExceptionFormat.Format(ex));
                ProcessFactory.KillCurrent();
                return string.Empty;
            }
        });
    }

    public void Remove(Uri uriForCachedItem)
    {
        try
        {
            File.Delete(ImageCacheFile.GetHashedFile(CacheFolder, uriForCachedItem));
        }
        catch
        {
            // Ignored
        }
    }

    public ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri)
    {
        return GetFileFromCacheAsync(uri, ElementTheme.Default);
    }

    public async ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri, ElementTheme theme)
    {
        Debug.Assert(SupportedSchemes.Contains(uri.Scheme), "Unsupported URI scheme");

        ImageCacheFile imageCacheFile = ImageCacheFile.Create(CacheFolder, uri);
        string themedFileFullPath = imageCacheFile.GetThemedFile(theme);

        using (await themeFileLocks.LockAsync((theme, imageCacheFile.HashedFileName)).ConfigureAwait(false))
        {
            if (IsFileInvalid(themedFileFullPath))
            {
                using (await downloadLocks.LockAsync(imageCacheFile.HashedFileName).ConfigureAwait(false))
                {
                    if (IsFileInvalid(imageCacheFile.DefaultFilePath))
                    {
                        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateInfo("Begin to download file", "Core.Caching.ImageCache", [("Uri", uri.ToString()), ("File", imageCacheFile.DefaultFilePath)]));

                        try
                        {
                            await downloadOperation.DownloadFileAsync(uri, imageCacheFile.DefaultFilePath).ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            Remove(uri);
                            throw;
                        }
                    }
                }
            }

            await EnsureThemedMonochromeFileExistsAsync(imageCacheFile, theme).ConfigureAwait(false);
            return themedFileFullPath;
        }
    }

    public ValueFile GetFileFromCategoryAndName(string category, string fileName)
    {
        return ImageCacheFile.Create(CacheFolder, StaticResourcesEndpoints.StaticRaw(category, fileName)).DefaultFilePath;
    }

    private static bool IsFileInvalid(string file, bool treatNullFileAsInvalid = true)
    {
        if (!File.Exists(file))
        {
            return treatNullFileAsInvalid;
        }

        return new FileInfo(file).Length == 0;
    }

    private static async ValueTask EnsureThemedMonochromeFileExistsAsync(ImageCacheFile imageCacheFile, ElementTheme theme)
    {
        if (theme is ElementTheme.Default)
        {
            return;
        }

        try
        {
            using (FileStream sourceStream = File.OpenRead(imageCacheFile.DefaultFilePath))
            {
                using (FileStream themeStream = File.Create(imageCacheFile.GetThemedFile(theme)))
                {
                    await MonoChromeImageConverter.ConvertAndCopyToAsync(theme, sourceStream, themeStream).ConfigureAwait(false);
                }
            }
        }
        catch (IOException ex)
        {
            throw InternalImageCacheException.Throw("Failed to convert image", ex);
        }
    }
}