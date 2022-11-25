// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.Model;
using System.Text;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 圣遗物套装
/// </summary>
public class ComplexReliquarySet
{
    /// <summary>
    /// 构造一个新的胡桃数据库圣遗物套装
    /// </summary>
    /// <param name="reliquarySetRate">圣遗物套装率</param>
    /// <param name="idReliquarySetMap">圣遗物套装映射</param>
    public ComplexReliquarySet(ItemRate<ReliquarySets, double> reliquarySetRate, Dictionary<EquipAffixId, Metadata.Reliquary.ReliquarySet> idReliquarySetMap)
    {
        ReliquarySets sets = reliquarySetRate.Item;

        if (sets.Count >= 1)
        {
            StringBuilder setStringBuilder = new();
            List<Uri> icons = new();
            foreach (ReliquarySet set in sets)
            {
                Metadata.Reliquary.ReliquarySet metaSet = idReliquarySetMap[set.EquipAffixId / 10];

                if (setStringBuilder.Length != 0)
                {
                    setStringBuilder.Append(Environment.NewLine);
                }

                setStringBuilder.Append(set.Count).Append('×').Append(metaSet.Name);
                icons.Add(RelicIconConverter.IconNameToUri(metaSet.Icon));
            }

            Name = setStringBuilder.ToString();
            Icons = icons;
        }
        else
        {
            Name = "无圣遗物或散件";
        }

        Rate = $"{reliquarySetRate.Rate:P3}";
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public List<Uri> Icons { get; set; } = default!;

    /// <summary>
    /// 比率
    /// </summary>
    public string Rate { get; set; } = default!;
}
