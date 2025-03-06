// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Web.Endpoint.Hutao;
using System.IO;
using System.Security.Cryptography;
using ThemeFile = (Microsoft.UI.Xaml.ElementTheme, Snap.Hutao.Core.IO.ValueFile);

namespace Snap.Hutao.Core.Caching;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IImageCache))]
internal sealed partial class ImageCache : IImageCache, IImageCacheFilePathOperation
{
    private readonly AsyncKeyedLock<ThemeFile> themeFileLocks = new();
    private readonly AsyncKeyedLock<string> downloadLocks = new();

    private readonly IImageCacheDownloadOperation downloadOperation;

    private string CacheFolder
    {
        get => LazyInitializer.EnsureInitialized(ref field, () =>
        {
            string folder = HutaoRuntime.GetLocalCacheImageCacheFolder();
            Directory.CreateDirectory(Path.Combine(folder, "Light"));
            Directory.CreateDirectory(Path.Combine(folder, "Dark"));
            return folder;
        });
    }

    public void Remove(Uri uriForCachedItem)
    {
        string filePath = Path.Combine(CacheFolder, CacheFile.GetCacheFileName(uriForCachedItem.OriginalString));
        try
        {
            File.Delete(filePath);
        }
        catch
        {
            // ignored
        }
    }

    public ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri)
    {
        return GetFileFromCacheAsync(uri, ElementTheme.Default);
    }

    public async ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri, ElementTheme theme)
    {
        // This method is performance critical, we potentially avoid logging when the file exists
        CacheFile cacheFile = CacheFile.Create(CacheFolder, uri);
        string themedFileFullPath = cacheFile.GetThemedFileFullPath(theme);

        if (IsFileValid(themedFileFullPath))
        {
            return themedFileFullPath;
        }

        using (await themeFileLocks.LockAsync((theme, cacheFile.FileName)).ConfigureAwait(false))
        {
            // If the file already exists, we don't need to download it again
            if (IsFileValid(cacheFile.DefaultFileFullPath))
            {
                await EnsureThemedMonochromeFileExistsAsync(cacheFile, theme).ConfigureAwait(false);
                return themedFileFullPath;
            }

            using (await downloadLocks.LockAsync(cacheFile.FileName).ConfigureAwait(false))
            {
                if (IsFileInvalid(cacheFile.DefaultFileFullPath))
                {
                    SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateInfo(
                        "Begin to download file",
                        "Core.Caching.ImageCache",
                        [("Uri", uri.ToString()), ("File", cacheFile.DefaultFileFullPath)]));

                    try
                    {
                        await downloadOperation.DownloadFileAsync(uri, cacheFile.DefaultFileFullPath).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Remove(uri);
                        SentrySdk.CaptureException(ex);
                    }
                }

                await EnsureThemedMonochromeFileExistsAsync(cacheFile, theme).ConfigureAwait(false);
                return themedFileFullPath;
            }
        }
    }

    public ValueFile GetFileFromCategoryAndName(string category, string fileName)
    {
        return CacheFile.Create(CacheFolder, StaticResourcesEndpoints.StaticRaw(category, fileName)).DefaultFileFullPath;
    }

    private static bool IsFileValid(string file, bool treatNullFileAsInvalid = true)
    {
        return !IsFileInvalid(file, treatNullFileAsInvalid);
    }

    private static bool IsFileInvalid(string file, bool treatNullFileAsInvalid = true)
    {
        if (!File.Exists(file))
        {
            return treatNullFileAsInvalid;
        }

        return new FileInfo(file).Length == 0;
    }

    private static async ValueTask EnsureThemedMonochromeFileExistsAsync(CacheFile cacheFile, ElementTheme theme)
    {
        if (theme is ElementTheme.Default)
        {
            return;
        }

        using (FileStream sourceStream = File.OpenRead(cacheFile.DefaultFileFullPath))
        {
            using (FileStream themeStream = File.Create(cacheFile.GetThemedFileFullPath(theme)))
            {
                await MonoChromeImageConverter.ConvertAndCopyToAsync(theme, sourceStream, themeStream).ConfigureAwait(false);
            }
        }
    }

    private sealed class CacheFile
    {
        private readonly string folder;

        private CacheFile(string folder, string fileName)
        {
            this.folder = folder;
            FileName = fileName;
        }

        public string FileName { get; }

        [field: MaybeNull]
        public string DefaultFileFullPath
        {
            get => field ??= Path.GetFullPath(Path.Combine(folder, FileName));
        }

        public static CacheFile Create(string folder, string url)
        {
            return new(folder, GetCacheFileName(url));
        }

        public static CacheFile Create(string folder, Uri uri)
        {
            return new(folder, GetCacheFileName(uri.OriginalString));
        }

        public static string GetCacheFileName(string url)
        {
            return Hash.ToHexString(HashAlgorithmName.SHA1, url);
        }

        public string GetThemedFileFullPath(ElementTheme theme)
        {
            return theme is ElementTheme.Default
                ? DefaultFileFullPath
                : Path.GetFullPath(Path.Combine(folder, $"{theme}", FileName));
        }
    }
}