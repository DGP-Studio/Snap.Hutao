﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Service.Game;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGachaLogQueryProvider), Key = RefreshOption.WebCache)]
internal sealed partial class GachaLogQueryWebCacheProvider : IGachaLogQueryProvider
{
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IGameServiceFacade gameService;
    private readonly CultureOptions cultureOptions;

    public static string GetCacheFile(string path)
    {
        string exeName = Path.GetFileName(path);
        string dataFolder = exeName == GameConstants.GenshinImpactFileName
            ? GameConstants.GenshinImpactData
            : GameConstants.YuanShenData;

        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        DirectoryInfo webCacheFolder = new(Path.Combine(directory, dataFolder, "webCaches"));
        if (webCacheFolder.Exists)
        {
            Regex versionRegex = VersionRegex();
            DirectoryInfo? lastestVersionCacheFolder = webCacheFolder
                .EnumerateDirectories()
                .Where(dir => versionRegex.IsMatch(dir.Name))
                .MaxBy(dir => new Version(dir.Name));

            lastestVersionCacheFolder ??= webCacheFolder;
            return Path.Combine(lastestVersionCacheFolder.FullName, @"Cache\Cache_Data\data_2");
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
        using (TempFileStream fileStream = TempFileStream.CopyFrom(cacheFile, FileMode.Open, FileAccess.Read))
        {
            using (MemoryStream memoryStream = await memoryStreamFactory.GetStreamAsync(fileStream).ConfigureAwait(false))
            {
                string? result = Match(memoryStream, cacheFile.Contains(GameConstants.GenshinImpactData, StringComparison.Ordinal));

                if (string.IsNullOrEmpty(result))
                {
                    return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderCacheUrlNotFound));
                }

                NameValueCollection query = HttpUtility.ParseQueryString(result.TrimEnd("#/log"));
                string? queryLanguageCode = query["lang"];

                if (!cultureOptions.LanguageCodeFitsCurrentLocale(queryLanguageCode))
                {
                    string message = SH.FormatServiceGachaLogUrlProviderUrlLanguageNotMatchCurrentLocale(queryLanguageCode, cultureOptions.LanguageCode);
                    return new(false, GachaLogQuery.Invalid(message));
                }

                return new(true, new(result));
            }
        }
    }

    private static string? Match(MemoryStream stream, bool isOversea)
    {
        ReadOnlySpan<byte> span = stream.ToArray();
        ReadOnlySpan<byte> match = isOversea
            ? "https://gs.hoyoverse.com/genshin/event/e20190909gacha-v3/index.html"u8
            : "https://webstatic.mihoyo.com/hk4e/event/e20190909gacha-v3/index.html"u8;

        int index = span.LastIndexOf(match);
        if (index >= 0)
        {
            index += match.Length;
            int length = span[index..].IndexOf("\0"u8);
            return Encoding.UTF8.GetString(span.Slice(index, length));
        }

        return null;
    }

    [GeneratedRegex("^[1-9]+?\\.[0-9]+?\\.[0-9]+?\\.[0-9]+?$")]
    private static partial Regex VersionRegex();
}