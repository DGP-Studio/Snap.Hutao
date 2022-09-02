// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 好感度信息
/// </summary>
public class FetterInfo
{
    /// <summary>
    /// 好感度等级
    /// </summary>
    [JsonPropertyName("expLevel")]
    public int ExpLevel { get; set; }
}