// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;

namespace Snap.Hutao.Service.Metadata;

internal interface IMetadataServiceNameDataMap
{
    /// <summary>
    /// 异步获取名称到角色的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>名称到角色的字典</returns>
    ValueTask<Dictionary<string, Avatar>> GetNameToAvatarMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取名称到武器的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>名称到武器的字典</returns>
    ValueTask<Dictionary<string, Weapon>> GetNameToWeaponMapAsync(CancellationToken token = default);
}
