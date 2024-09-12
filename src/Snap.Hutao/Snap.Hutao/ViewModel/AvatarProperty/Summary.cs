// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 玩家与角色列表的包装器
/// </summary>
[HighQuality]
internal sealed class Summary
{
    public AdvancedCollectionView<AvatarView> Avatars { get; set; } = default!;

    public string Message { get; set; } = default!;
}