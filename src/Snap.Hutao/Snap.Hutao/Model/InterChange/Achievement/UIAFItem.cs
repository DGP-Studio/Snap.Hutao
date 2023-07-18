// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.InterChange.Achievement;

/// <summary>
/// UIAF 项
/// </summary>
internal sealed class UIAFItem : IMappingFrom<UIAFItem, Entity.Achievement>
{
    /// <summary>
    /// 成就Id
    /// </summary>
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// 当前值
    /// 对于progress为1的项，该属性始终为0
    /// </summary>
    [JsonPropertyName("current")]
    public uint Current { get; set; }

    /// <summary>
    /// 完成状态
    /// </summary>
    [JsonPropertyName("status")]
    public AchievementStatus Status { get; set; }

    public static UIAFItem From(Entity.Achievement source)
    {
        return new()
        {
            Id = source.Id,
            Current = source.Current,
            Status = source.Status,
            Timestamp = source.Time.ToUnixTimeSeconds(),
        };
    }
}