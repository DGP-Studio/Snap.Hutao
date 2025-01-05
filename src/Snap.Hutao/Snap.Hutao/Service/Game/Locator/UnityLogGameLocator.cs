// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Game.Locator;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGameLocator), Key = GameLocationSourceKind.UnityLog)]
internal sealed partial class UnityLogGameLocator : IGameLocator, IGameLocator2
{
    private readonly ITaskContext taskContext;

    [GeneratedRegex(@".:/.+(?:GenshinImpact|YuanShen)(?=_Data)", RegexOptions.IgnoreCase)]
    private static partial Regex WarmupFileLine { get; }

    public async ValueTask<ValueResult<bool, string>> LocateSingleGamePathAsync()
    {
        await taskContext.SwitchToBackgroundAsync();

        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string logFilePathOversea = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\Genshin Impact\output_log.txt");
        string logFilePathChinese = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\原神\output_log.txt");

        // Fallback to the CN server.
        if (await LocateGamePathAsync(logFilePathOversea).ConfigureAwait(false) is { IsOk: true } resultOversea)
        {
            return resultOversea;
        }

        return await LocateGamePathAsync(logFilePathChinese).ConfigureAwait(false);
    }

    public async ValueTask<ImmutableArray<string>> LocateMultipleGamePathAsync()
    {
        await taskContext.SwitchToBackgroundAsync();

        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string logFilePathOversea = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\Genshin Impact\output_log.txt");
        string logFilePathChinese = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\原神\output_log.txt");

        ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>(2);

        if (File.Exists(logFilePathOversea))
        {
            ValueResult<bool, string> result = await LocateGamePathAsync(logFilePathOversea).ConfigureAwait(false);
            if (result.IsOk)
            {
                builder.Add(result.Value);
            }
        }

        if (File.Exists(logFilePathChinese))
        {
            ValueResult<bool, string> result = await LocateGamePathAsync(logFilePathChinese).ConfigureAwait(false);
            if (result.IsOk)
            {
                builder.Add(result.Value);
            }
        }

        return builder.ToImmutable();
    }

    private static async ValueTask<ValueResult<bool, string>> LocateGamePathAsync(string logFilePath)
    {
        if (!File.Exists(logFilePath))
        {
            return new(false, SH.ServiceGameLocatorUnityLogFileNotFound);
        }

        string content;
        try
        {
            content = await File.ReadAllTextAsync(logFilePath).ConfigureAwait(false);
        }
        catch (IOException)
        {
            return new(false, SH.ServiceGameLocatorUnityLogCannotOpenRead);
        }

        Match matchResult = WarmupFileLine.Match(content);
        if (!matchResult.Success)
        {
            return new(false, SH.ServiceGameLocatorUnityLogGamePathNotFound);
        }

        string entryName = $"{matchResult.Value}.exe";
        string fullPath = Path.GetFullPath(Path.Combine(matchResult.Value, "..", entryName));
        if (!File.Exists(fullPath))
        {
            return new(false, default!);
        }

        return new(true, fullPath);
    }
}