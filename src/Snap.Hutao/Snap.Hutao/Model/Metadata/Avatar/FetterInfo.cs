// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 好感信息
/// </summary>
[HighQuality]
internal sealed class FetterInfo
{
    /// <summary>
    /// 称号
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// 详细
    /// </summary>
    public string Detail { get; set; } = default!;

    /// <summary>
    /// 地区
    /// </summary>
    [JsonEnum(JsonSerializeType.String)]
    public AssociationType Association { get; set; } = default!;

    /// <summary>
    /// 属于组织
    /// </summary>
    public string Native { get; set; } = default!;

    /// <summary>
    /// 生月
    /// </summary>
    public int BirthMonth { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public int BirthDay { get; set; }

    /// <summary>
    /// 格式化的生日日期
    /// </summary>
    public string BirthFormatted
    {
        get => string.Format(SH.ModelMetadataFetterInfoBirthdayFormat, BirthMonth, BirthDay);
    }

    /// <summary>
    /// 神之眼属性-前
    /// </summary>
    public string VisionBefore { get; set; } = default!;

    /// <summary>
    /// 命座-前
    /// </summary>
    public string ConstellationBefore { get; set; } = default!;

    /// <summary>
    /// 命座-后
    /// </summary>
    public string ConstellationAfter { get; set; } = default!;

    /// <summary>
    /// 命座
    /// </summary>
    public string Constellation
    {
        get
        {
            if (string.IsNullOrEmpty(ConstellationAfter))
            {
                return ConstellationBefore;
            }
            else
            {
                return ConstellationAfter;
            }
        }
    }

    /// <summary>
    /// 中文CV
    /// </summary>
    public string CvChinese { get; set; } = default!;

    /// <summary>
    /// 日语CV
    /// </summary>
    public string CvJapanese { get; set; } = default!;

    /// <summary>
    /// 英语CV
    /// </summary>
    public string CvEnglish { get; set; } = default!;

    /// <summary>
    /// 韩语CV
    /// </summary>
    public string CvKorean { get; set; } = default!;

    /// <summary>
    /// 料理
    /// </summary>
    public CookBonus? CookBonus { get; set; }

    /// <summary>
    /// 好感语音
    /// </summary>
    public IEnumerable<Fetter> Fetters { get; set; } = default!;

    /// <summary>
    /// 好感故事
    /// </summary>
    public IEnumerable<Fetter> FetterStories { get; set; } = default!;
}