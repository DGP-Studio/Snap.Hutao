// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using System.IO;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// Unity 日志游戏定位器
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGameLocator), Key = GameLocationSource.UnityLog)]
internal sealed partial class UnityLogGameLocator : IGameLocator
{
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> LocateGamePathAsync()
    {
        await taskContext.SwitchToBackgroundAsync();

        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string logFilePathChinese = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\原神\output_log.txt");
        string logFilePathOversea = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\Genshin Impact\output_log.txt");

        // Fallback to the CN server.
        string logFilePath = File.Exists(logFilePathOversea) ? logFilePathOversea : logFilePathChinese;

        if (!File.Exists(logFilePath))
        {
            return new(false, SH.ServiceGameLocatorUnityLogFileNotFound);
        }

        string content = await File.ReadAllTextAsync(logFilePath).ConfigureAwait(false);

        Match matchResult = WarmupFileLine().Match(content);
        if (!matchResult.Success)
        {
            return new(false, SH.ServiceGameLocatorUnityLogGamePathNotFound);
        }

        string entryName = $"{matchResult.Value}.exe";
        string fullPath = Path.GetFullPath(Path.Combine(matchResult.Value, "..", entryName));
        return new(true, fullPath);
    }

    [GeneratedRegex(@".:/.+(?:GenshinImpact|YuanShen)(?=_Data)")]
    private static partial Regex WarmupFileLine();
}