// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 参量质变仪恢复时间包装
/// 已准备完成 $后可再次使用
/// 冷却中     可使用
/// </summary>
public class RecoveryTime
{
    /// <summary>
    /// 日
    /// </summary>
    [JsonPropertyName("Day")]
    public int Day { get; set; }

    /// <summary>
    /// 时
    /// </summary>
    [JsonPropertyName("Hour")]
    public int Hour { get; set; }

    /// <summary>
    /// 分
    /// </summary>
    [JsonPropertyName("Minute")]
    public int Minute { get; set; }

    /// <summary>
    /// 秒
    /// </summary>
    [JsonPropertyName("Second")]
    public int Second { get; set; }

    /// <summary>
    /// 是否已经到达
    /// </summary>
    [JsonPropertyName("reached")]
    public bool Reached { get; set; }

    /// <summary>
    /// 获取格式化的剩余时间
    /// </summary>
    public string TimeFormatted
    {
        get
        {
            if (Reached)
            {
                return "已准备完成";
            }
            else
            {
                return new StringBuilder()
                    .AppendIf(Day > 0, $"{Day}天")
                    .AppendIf(Hour > 0, $"{Hour}时")
                    .AppendIf(Minute > 0, $"{Minute}分")
                    .AppendIf(Second > 0, $"{Second}秒")
                    .Append(" 后可再次使用")
                    .ToString();
            }
        }
    }

    /// <summary>
    /// 获取格式化的状态
    /// </summary>
    public string ReachedFormatted
    {
        get => Reached ? "可使用" : "冷却中";
    }
}