// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.Process;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Win32;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using ThemeFile = (Microsoft.UI.Xaml.ElementTheme, Snap.Hutao.Core.IO.ValueFile);

namespace Snap.Hutao.Core.Caching;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IImageCache))]
internal sealed partial class ImageCache : IImageCache, IImageCacheFilePathOperation
{
    private readonly AsyncKeyedLock<ThemeFile> themeFileLocks = new();
    private readonly AsyncKeyedLock<string> downloadLocks = new();

    private readonly IImageCacheDownloadOperation downloadOperation;

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
        string filePath = Path.Combine(CacheFolder, CacheFile.GetHashedFileName(uriForCachedItem.OriginalString));
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
        string themedFileFullPath = cacheFile.GetThemedFilePath(theme);

        using (await themeFileLocks.LockAsync((theme, cacheFile.HashedFileName)).ConfigureAwait(false))
        {
            if (IsFileInvalid(themedFileFullPath))
            {
                using (await downloadLocks.LockAsync(cacheFile.HashedFileName).ConfigureAwait(false))
                {
                    if (IsFileInvalid(cacheFile.DefaultFilePath))
                    {
                        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateInfo("Begin to download file", "Core.Caching.ImageCache", [("Uri", uri.ToString()), ("File", cacheFile.DefaultFilePath)]));

                        try
                        {
                            await downloadOperation.DownloadFileAsync(uri, cacheFile.DefaultFilePath).ConfigureAwait(false);
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
        return CacheFile.Create(CacheFolder, StaticResourcesEndpoints.StaticRaw(category, fileName)).DefaultFilePath;
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
            using (FileStream sourceStream = File.OpenRead(cacheFile.DefaultFilePath))
            {
                using (FileStream themeStream = File.Create(cacheFile.GetThemedFilePath(theme)))
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
        private readonly string directory;

        private CacheFile(string directory, string hashedFileName)
        {
            this.directory = directory;
            HashedFileName = hashedFileName;
        }

        public string HashedFileName { get; }

        [field: MaybeNull]
        public string DefaultFilePath
        {
            get => field ??= Path.GetFullPath(Path.Combine(directory, HashedFileName));
        }

        public static CacheFile Create(string folder, string url)
        {
            return new(folder, GetHashedFileName(url));
        }

        public static CacheFile Create(string folder, Uri uri)
        {
            return new(folder, GetHashedFileName(uri.OriginalString));
        }

        public static string GetHashedFileName(string url)
        {
            return Hash.ToHexString(HashAlgorithmName.SHA1, url);
        }

        public string GetThemedFilePath(ElementTheme theme)
        {
            return theme is ElementTheme.Default
                ? DefaultFilePath
                : Path.GetFullPath(Path.Combine(directory, $"{theme}", HashedFileName));
        }
    }
}