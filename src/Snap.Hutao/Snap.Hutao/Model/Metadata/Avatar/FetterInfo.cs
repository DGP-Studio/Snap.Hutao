// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 好感信息
/// </summary>
public class FetterInfo
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
    public string Association { get; set; } = default!;

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
        get => $"{BirthMonth} 月 {BirthDay} 日";
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
    /// 好感语音
    /// </summary>
    public IEnumerable<Fetter> Fetters { get; set; } = default!;

    /// <summary>
    /// 好感故事
    /// </summary>
    public IEnumerable<Fetter> FetterStories { get; set; } = default!;
}