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
        MetadataReliquary metaReliquary = metadataContext.IdReliquaryMap[reliquary.Id];
        MetadataReliquarySet metaReliquarySet = metadataContext.IdReliquarySetMap[metaReliquary.SetId];

        ReliquaryViewBuilder reliquaryViewBuilder = new ReliquaryViewBuilder()
            .SetName(metaReliquary.Name)
            .SetIcon(RelicIconConverter.IconNameToUri(metaReliquary.Icon))
            .SetDescription(metaReliquary.Description)
            .SetLevel($"+{reliquary.Level}")
            .SetQuality(metaReliquary.RankLevel)
            .SetEquipType(metaReliquary.EquipType)
            .SetSetName(metaReliquarySet.Name)
            .SetMainProperty(FightPropertyFormat.ToNameValue(reliquary.MainProperty))
            .SetComposedSubProperties(reliquary.SubPropertyList.SelectAsArray(CreateSubProperty));

        return reliquaryViewBuilder.View;
    }

    private static ReliquaryComposedSubProperty CreateSubProperty(ReliquaryProperty property)
    {
        return new(property.PropertyType, property.Value, property.Times + 1);
    }
}