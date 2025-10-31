// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Game.Locator;

[Service(ServiceLifetime.Transient, typeof(IGameLocator), Key = GameLocationSourceKind.UnityLog)]
internal sealed partial class UnityLogGameLocator : IGameLocator, IGameLocator2
{
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial UnityLogGameLocator(IServiceProvider serviceProvider);

    [GeneratedRegex(@".:(?:\\|/).+(?:GenshinImpact|YuanShen)(?=_Data)", RegexOptions.IgnoreCase)]
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

        if (await LocateGamePathAsync(logFilePathOversea).ConfigureAwait(false) is { IsOk: true } result1)
        {
            builder.Add(result1.Value);
        }

        if (await LocateGamePathAsync(logFilePathChinese).ConfigureAwait(false) is { IsOk: true } result2)
        {
            builder.Add(result2.Value);
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

        if (WarmupFileLine.Match(content) is not { Success: true } matchResult)
        {
            return new(false, SH.ServiceGameLocatorUnityLogGamePathNotFound);
        }

        string fullPath = Path.GetFullPath($"{matchResult.Value}.exe");

        if (!File.Exists(fullPath))
        {
            return new(false, default!);
        }

        return new(true, fullPath);
    }
}