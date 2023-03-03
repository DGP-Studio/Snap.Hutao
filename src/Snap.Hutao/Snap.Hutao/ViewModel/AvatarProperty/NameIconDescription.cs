// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 名称与描述与图标抽象
/// </summary>
[HighQuality]
internal abstract class NameIconDescription : NameDescription, INameIcon
{
    /// <summary>
    /// 图标
    /// </summary>
    public Uri Icon { get; set; } = default!;
}