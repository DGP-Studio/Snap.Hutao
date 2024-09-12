// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Service.AvatarInfo.Factory.Builder;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using DetailedReliquary = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar.Reliquary;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataReliquarySet = Snap.Hutao.Model.Metadata.Reliquary.ReliquarySet;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

[HighQuality]
internal sealed class SummaryReliquaryFactory
{
    private readonly SummaryFactoryMetadataContext metadataContext;
    private readonly DetailedReliquary reliquary;

    public SummaryReliquaryFactory(SummaryFactoryMetadataContext metadataContext, DetailedReliquary reliquary)
    {
        this.metadataContext = metadataContext;
        this.reliquary = reliquary;
    }

    public static ReliquaryView Create(SummaryFactoryMetadataContext metadataContext, DetailedReliquary reliquary)
    {
        return new SummaryReliquaryFactory(metadataContext, reliquary).Create();
    }

    public ReliquaryView Create()
    {
        MetadataReliquary reliquary = metadataContext.IdReliquaryMap[this.reliquary.Id];
        MetadataReliquarySet reliquarySet = metadataContext.IdReliquarySetMap[reliquary.SetId];

        ReliquaryViewBuilder reliquaryViewBuilder = new ReliquaryViewBuilder()
            .SetName(reliquary.Name)
            .SetIcon(RelicIconConverter.IconNameToUri(reliquary.Icon))
            .SetDescription(reliquary.Description)
            .SetLevel($"+{this.reliquary.Level}")
            .SetQuality(reliquary.RankLevel)
            .SetEquipType(reliquary.EquipType)
            .SetSetName(reliquarySet.Name)
            .SetMainProperty(FightPropertyFormat.ToNameValue(this.reliquary.MainProperty))
            .SetComposedSubProperties(this.reliquary.SubPropertyList.SelectList(CreateSubProperty));

        return reliquaryViewBuilder.View;
    }

    private ReliquaryComposedSubProperty CreateSubProperty(ReliquaryProperty property)
    {
        return new ReliquaryComposedSubProperty(property.PropertyType, property.Value)
        {
            EnhancedCount = property.Times + 1,
        };
    }
}