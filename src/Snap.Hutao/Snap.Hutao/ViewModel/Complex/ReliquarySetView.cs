// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Collections.Immutable;
using System.Text;

namespace Snap.Hutao.ViewModel.Complex;

/// <summary>
/// 圣遗物套装
/// </summary>
internal sealed class ReliquarySetView : RateAndDelta
{
    public ReliquarySetView(ImmutableDictionary<ExtendedEquipAffixId, Model.Metadata.Reliquary.ReliquarySet> idReliquarySetMap, ItemRate<ReliquarySets, double> setRate, ItemRate<ReliquarySets, double>? lastSetRate)
        : base(setRate.Rate, lastSetRate?.Rate)
    {
        ReliquarySets sets = setRate.Item;

        if (sets is [_, ..])
        {
            StringBuilder nameBuilder = new();
            List<Uri> icons = new(2);
            foreach (ReliquarySet set in sets)
            {
                Model.Metadata.Reliquary.ReliquarySet metaSet = idReliquarySetMap[set.EquipAffixId];
                nameBuilder.Append(set.Count).Append('×').Append(metaSet.Name).Append('+');
                icons.Add(RelicIconConverter.IconNameToUri(metaSet.Icon));
            }

            Name = nameBuilder.ToString(0, nameBuilder.Length - 1);
            Icons = icons;
        }
        else
        {
            Name = SH.ViewModelComplexReliquarySetViewEmptyName;
        }
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 图标
    /// </summary>
    public List<Uri>? Icons { get; }
}