// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.Wiki;

/// <summary>
/// 料理奖励视图
/// </summary>
[HighQuality]
internal sealed class CookBonusView
{
    /// <summary>
    /// 原型
    /// </summary>
    public Material OriginItem { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public Material Item { get; set; } = default!;

    /// <summary>
    /// 创建一个新的料理奖励视图
    /// </summary>
    /// <param name="cookBonus">料理奖励</param>
    /// <param name="idMaterialMap">材料映射</param>
    /// <returns>新的料理奖励视图</returns>
    public static CookBonusView? Create(CookBonus? cookBonus, Dictionary<MaterialId, Material> idMaterialMap)
    {
        if (cookBonus is null)
        {
            return null;
        }

        CookBonusView view = new()
        {
            OriginItem = idMaterialMap[cookBonus.OriginItemId],
            Item = idMaterialMap[cookBonus.ItemId],
        };

        return view;
    }
}