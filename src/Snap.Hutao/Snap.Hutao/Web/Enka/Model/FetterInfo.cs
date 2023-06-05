// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 好感度信息
/// </summary>
[HighQuality]
internal sealed class FetterInfo
{
    /// <summary>
    /// 好感度等级
    /// </summary>
    [JsonPropertyName("expLevel")]
    public FetterLevel ExpLevel { get; set; }
}