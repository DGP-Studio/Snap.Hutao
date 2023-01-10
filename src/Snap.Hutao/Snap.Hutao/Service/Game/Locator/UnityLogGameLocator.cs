// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using System.IO;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// Unity日志游戏定位器
/// </summary>
[Injection(InjectAs.Transient, typeof(IGameLocator))]
internal partial class UnityLogGameLocator : IGameLocator
{
    /// <inheritdoc/>
    public string Name { get => nameof(UnityLogGameLocator); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> LocateGamePathAsync()
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string logFilePathChinese = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\原神\output_log.txt");
        string logFilePathOvsesea = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\Genshin Impact\output_log.txt");

        // We need to fallback the the cn server rather than os server.
        string logFilePathFinal = File.Exists(logFilePathOvsesea) ? logFilePathOvsesea : logFilePathChinese;

        using (TempFile? tempFile = TempFile.CreateFromFileCopy(logFilePathFinal))
        {
            if (tempFile != null)
            {
                string content = File.ReadAllText(tempFile.Path);

                Match matchResult = WarmupFileLine().Match(content);
                if (!matchResult.Success)
                {
                    return new(false, $"在 Unity 日志文件中找不到游戏路径");
                }

                string entryName = matchResult.Groups[0].Value.Replace("_Data", ".exe");
                string fullPath = Path.GetFullPath(Path.Combine(matchResult.Value, "..", entryName));
                return new(true, fullPath);
            }
            else
            {
                return new(false, $"找不到 Unity 日志文件");
            }
        }
    }

    [GeneratedRegex(@"(?m).:/.+(GenshinImpact_Data|YuanShen_Data)")]
    private static partial Regex WarmupFileLine();
}