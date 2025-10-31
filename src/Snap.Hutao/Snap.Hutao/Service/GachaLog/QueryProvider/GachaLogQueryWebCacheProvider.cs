// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Service.Game;
using System.Buffers;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

[Service(ServiceLifetime.Transient, typeof(IGachaLogQueryProvider), Key = RefreshOptionKind.WebCache)]
internal sealed partial class GachaLogQueryWebCacheProvider : IGachaLogQueryProvider
{
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IGameService gameService;
    private readonly CultureOptions cultureOptions;

    [GeneratedConstructor]
    public partial GachaLogQueryWebCacheProvider(IServiceProvider serviceProvider);

    [GeneratedRegex("^[1-9]+?\\.[0-9]+?\\.[0-9]+?\\.[0-9]+?$")]
    private static partial Regex VersionRegex { get; }

    public static string GetCacheFile(string path)
    {
        string exeName = Path.GetFileName(path);
        string dataFolder = exeName is GameConstants.GenshinImpactFileName
            ? GameConstants.GenshinImpactData
            : GameConstants.YuanShenData;

        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        DirectoryInfo webCacheFolder = new(Path.Combine(directory, dataFolder, "webCaches"));
        if (webCacheFolder.Exists)
        {
            DirectoryInfo? latestVersionCacheFolder = webCacheFolder
                .EnumerateDirectories()
                .Where(dir => VersionRegex.IsMatch(dir.Name))
                .MaxBy(dir => new Version(dir.Name));

            latestVersionCacheFolder ??= webCacheFolder;
            return Path.Combine(latestVersionCacheFolder.FullName, @"Cache\Cache_Data\data_2");
        }

        return string.Empty;
    }

    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        (bool isOk, string path) = await gameService.GetGamePathAsync().ConfigureAwait(false);
        if (!isOk || string.IsNullOrEmpty(path))
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderCachePathInvalid));
        }

        string cacheFile = GetCacheFile(path);
        if (!File.Exists(cacheFile))
        {
            return new(false, GachaLogQuery.Invalid(SH.FormatServiceGachaLogUrlProviderCachePathNotFound(cacheFile)));
        }

        // Must copy the file to avoid the following exception:
        // System.IO.IOException: The process cannot access the file
        TempFileStream fileStream;
        try
        {
            fileStream = TempFileStream.CopyFrom(cacheFile, FileMode.Open, FileAccess.Read);
        }
        catch (UnauthorizedAccessException)
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderCopyWebCacheUnauthorizedAccess));
        }

        using (fileStream)
        {
            using (MemoryStream memoryStream = await memoryStreamFactory.GetStreamAsync(fileStream).ConfigureAwait(false))
            {
                string? result = Match(memoryStream, cacheFile.Contains(GameConstants.GenshinImpactData, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(result))
                {
                    return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderCacheUrlNotFound));
                }

                NameValueCollection query = HttpUtility.ParseQueryString(result.TrimEnd("#/log"));
                string? queryLanguageCode = query["lang"];

                if (!LocaleNames.LanguageCodeFitsCurrentLocale(queryLanguageCode, cultureOptions.LocaleName))
                {
                    string message = SH.FormatServiceGachaLogUrlProviderUrlLanguageNotMatchCurrentLocale(queryLanguageCode, cultureOptions.LanguageCode);
                    return new(false, GachaLogQuery.Invalid(message));
                }

                return new(true, new(result));
            }
        }
    }

    private static unsafe string? Match(MemoryStream stream, bool isOversea)
    {
        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.RentExactly((int)stream.Length))
        {
            Span<byte> span = memoryOwner.Memory.Span;
            stream.ReadExactly(span);

            ReadOnlySpan<byte> match = isOversea
                ? "https://gs.hoyoverse.com/genshin/event/e20190909gacha-v3/index.html"u8
                : "https://webstatic.mihoyo.com/hk4e/event/e20190909gacha-v3/index.html"u8;

            int index = span.LastIndexOf(match);
            if (index >= 0)
            {
                index += match.Length;

                ref byte reference = ref span[index];
                fixed (byte* ptr = &reference)
                {
                    ReadOnlySpan<byte> target = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr);
                    return Encoding.UTF8.GetString(target);
                }
            }

            return null;
        }
    }
}