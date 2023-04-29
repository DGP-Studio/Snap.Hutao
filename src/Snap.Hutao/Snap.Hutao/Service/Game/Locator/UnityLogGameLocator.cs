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
[Injection(InjectAs.Transient, typeof(IGameLocator))]
internal sealed partial class UnityLogGameLocator : IGameLocator
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的 Unity 日志游戏定位器
    /// </summary>
    /// <param name="taskContext">任务上下文</param>
    public UnityLogGameLocator(ITaskContext taskContext)
    {
        this.taskContext = taskContext;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(UnityLogGameLocator); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> LocateGamePathAsync()
    {
        await taskContext.SwitchToBackgroundAsync();

        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string logFilePathChinese = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\原神\output_log.txt");
        string logFilePathOvsesea = Path.Combine(appDataPath, @"..\LocalLow\miHoYo\Genshin Impact\output_log.txt");

        // Fallback to the CN server.
        string logFilePathFinal = File.Exists(logFilePathOvsesea) ? logFilePathOvsesea : logFilePathChinese;

        using (TempFile? tempFile = TempFile.CopyFrom(logFilePathFinal))
        {
            if (tempFile != null)
            {
                string content = File.ReadAllText(tempFile.Path);

                Match matchResult = WarmupFileLine().Match(content);
                if (!matchResult.Success)
                {
                    return new(false, SH.ServiceGameLocatorUnityLogGamePathNotFound);
                }

                string entryName = matchResult.Groups[0].Value.Replace("_Data", ".exe");
                string fullPath = Path.GetFullPath(Path.Combine(matchResult.Value, "..", entryName));
                return new(true, fullPath);
            }
            else
            {
                return new(false, SH.ServiceGameLocatorUnityLogFileNotFound);
            }
        }
    }

    [GeneratedRegex(@"(?m).:/.+(GenshinImpact_Data|YuanShen_Data)")]
    private static partial Regex WarmupFileLine();
}