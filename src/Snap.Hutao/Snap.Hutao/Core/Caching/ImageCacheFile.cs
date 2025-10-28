// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using System.IO;
using System.Security.Cryptography;

namespace Snap.Hutao.Core.Caching;

internal sealed class ImageCacheFile
{
    private readonly string directory;

    private ImageCacheFile(ValueDirectory directory, string hashedFileName)
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

    public static ImageCacheFile Create(ValueDirectory folder, string url)
    {
        return new(folder, GetHashedFileName(url));
    }

    public static ImageCacheFile Create(ValueDirectory folder, Uri uri)
    {
        return new(folder, GetHashedFileName(uri.OriginalString));
    }

    public static ValueFile GetHashedFile(ValueDirectory folder, string url)
    {
        return Path.GetFullPath(Path.Combine(folder, GetHashedFileName(url)));
    }

    public static ValueFile GetHashedFile(ValueDirectory folder, Uri uri)
    {
        return Path.GetFullPath(Path.Combine(folder, GetHashedFileName(uri.OriginalString)));
    }

    public static string GetHashedFileName(string url)
    {
        return Hash.ToHexString(HashAlgorithmName.SHA1, url);
    }

    public ValueFile GetThemedFile(ElementTheme theme)
    {
        return theme is ElementTheme.Default
            ? DefaultFilePath
            : Path.GetFullPath(Path.Combine(directory, $"{theme}", HashedFileName));
    }
}