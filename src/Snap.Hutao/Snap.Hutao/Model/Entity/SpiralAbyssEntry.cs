// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 深渊记录入口点
/// </summary>
[HighQuality]
[Table("spiral_abysses")]
internal sealed class SpiralAbyssEntry : ObservableObject,
    IMappingFrom<SpiralAbyssEntry, string, Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss>
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 计划Id
    /// </summary>
    public uint ScheduleId { get; set; }

    /// <summary>
    /// Uid
    /// </summary>
    public string Uid { get; set; } = default!;

    /// <summary>
    /// Json!!! 深渊记录
    /// </summary>
    public Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss SpiralAbyss { get; set; } = default!;

    /// <summary>
    /// 创建一个新的深渊信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="spiralAbyss">深渊信息</param>
    /// <returns>新的深渊信息</returns>
    public static SpiralAbyssEntry From(string uid, Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss)
    {
        return new()
        {
            Uid = uid,
            ScheduleId = spiralAbyss.ScheduleId,
            SpiralAbyss = spiralAbyss,
        };
    }
}