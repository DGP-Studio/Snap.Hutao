// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 深渊记录入口点
/// </summary>
[HighQuality]
[Table("spiral_abysses")]
internal sealed class SpiralAbyssEntry : ObservableObject
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
    public int ScheduleId { get; set; }

    /// <summary>
    /// 视图 中使用的计划 Id 字符串
    /// </summary>
    [NotMapped]
    public string Schedule { get => string.Format(SH.ModelEntitySpiralAbyssScheduleFormat, ScheduleId); }

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
    public static SpiralAbyssEntry Create(string uid, Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss)
    {
        return new()
        {
            Uid = uid,
            ScheduleId = spiralAbyss.ScheduleId,
            SpiralAbyss = spiralAbyss,
        };
    }

    /// <summary>
    /// 更新深渊信息
    /// </summary>
    /// <param name="spiralAbyss">深渊信息</param>
    public void UpdateSpiralAbyss(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss)
    {
        SpiralAbyss = spiralAbyss;
        OnPropertyChanged(nameof(SpiralAbyss));
    }
}