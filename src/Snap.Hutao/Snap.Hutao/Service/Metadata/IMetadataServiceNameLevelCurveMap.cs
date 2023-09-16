// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata;

internal interface IMetadataServiceNameLevelCurveMap
{
    /// <summary>
    /// 异步获取等级角色曲线映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>等级角色曲线映射</returns>
    ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToAvatarCurveMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取等级怪物曲线映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>等级怪物曲线映射</returns>
    ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToMonsterCurveMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取等级武器曲线映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>等级武器曲线映射</returns>
    ValueTask<Dictionary<Level, Dictionary<GrowCurveType, float>>> GetLevelToWeaponCurveMapAsync(CancellationToken token = default);
}