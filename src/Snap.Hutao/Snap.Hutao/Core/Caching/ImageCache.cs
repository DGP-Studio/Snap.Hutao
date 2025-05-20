// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Web.Endpoint.Hutao;
using System.Diagnostics;
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
        get => LazyInitializer.EnsureInitialized(ref field, static () =>
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
            // Ignored
        }
    }

    public ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri)
    {
        return GetFileFromCacheAsync(uri, ElementTheme.Default);
    }

    public async ValueTask<ValueFile> GetFileFromCacheAsync(Uri uri, ElementTheme theme)
    {
        Debug.Assert(uri.Scheme is "http" or "https", "Unsupported URI scheme");

        CacheFile cacheFile = CacheFile.Create(CacheFolder, uri);
        string themedFileFullPath = cacheFile.GetThemedFileFullPath(theme);

        using (await themeFileLocks.LockAsync((theme, cacheFile.FileName)).ConfigureAwait(false))
        {
            if (IsFileInvalid(themedFileFullPath))
            {
                using (await downloadLocks.LockAsync(cacheFile.FileName).ConfigureAwait(false))
                {
                    if (IsFileInvalid(cacheFile.DefaultFileFullPath))
                    {
                        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateInfo("Begin to download file", "Core.Caching.ImageCache", [("Uri", uri.ToString()), ("File", cacheFile.DefaultFileFullPath)]));

                        try
                        {
                            await downloadOperation.DownloadFileAsync(uri, cacheFile.DefaultFileFullPath).ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            Remove(uri);
                            throw;
                        }
                    }
                }
            }

            await EnsureThemedMonochromeFileExistsAsync(cacheFile, theme).ConfigureAwait(false);
            return themedFileFullPath;
        }
    }

    public ValueFile GetFileFromCategoryAndName(string category, string fileName)
    {
        return CacheFile.Create(CacheFolder, StaticResourcesEndpoints.StaticRaw(category, fileName)).DefaultFileFullPath;
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

        try
        {
            using (FileStream sourceStream = File.OpenRead(cacheFile.DefaultFileFullPath))
            {
                using (FileStream themeStream = File.Create(cacheFile.GetThemedFileFullPath(theme)))
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