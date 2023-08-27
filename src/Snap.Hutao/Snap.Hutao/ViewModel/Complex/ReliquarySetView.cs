// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.ViewModel.Complex;

/// <summary>
/// 圣遗物套装
/// </summary>
internal sealed class ReliquarySetView
{
    /// <summary>
    /// 构造一个新的胡桃数据库圣遗物套装
    /// </summary>
    /// <param name="reliquarySetRate">圣遗物套装率</param>
    /// <param name="idReliquarySetMap">圣遗物套装映射</param>
    public ReliquarySetView(ItemRate<ReliquarySets, double> reliquarySetRate, Dictionary<ExtendedEquipAffixId, Model.Metadata.Reliquary.ReliquarySet> idReliquarySetMap)
    {
        ReliquarySets sets = reliquarySetRate.Item;

        if (!sets.IsNullOrEmpty())
        {
            StringBuilder nameBuilder = new();
            List<Uri> icons = new(2);
            foreach (ReliquarySet set in CollectionsMarshal.AsSpan(sets))
            {
                Model.Metadata.Reliquary.ReliquarySet metaSet = idReliquarySetMap[set.EquipAffixId];

                if (nameBuilder.Length > 0)
                {
                    nameBuilder.AppendLine();
                }

                nameBuilder.Append(set.Count).Append('×').Append(metaSet.Name);
                icons.Add(RelicIconConverter.IconNameToUri(metaSet.Icon));
            }

            Name = nameBuilder.ToString();
            Icons = icons;
        }
        else
        {
            Name = SH.ViewModelComplexReliquarySetViewEmptyName;
        }

        Rate = $"{reliquarySetRate.Rate:P3}";
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 图标
    /// </summary>
    public List<Uri>? Icons { get; }

    /// <summary>
    /// 比率
    /// </summary>
    public string Rate { get; }
}
