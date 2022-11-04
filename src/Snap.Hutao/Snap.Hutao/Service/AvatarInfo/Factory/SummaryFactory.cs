// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Service.Metadata;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using ModelPlayerInfo = Snap.Hutao.Web.Enka.Model.PlayerInfo;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述工厂
/// </summary>
[Injection(InjectAs.Transient, typeof(ISummaryFactory))]
internal class SummaryFactory : ISummaryFactory
{
    private readonly IMetadataService metadataService;

    /// <summary>
    /// 构造一个新的简述工厂
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    public SummaryFactory(IMetadataService metadataService)
    {
        this.metadataService = metadataService;
    }

    /// <inheritdoc/>
    public async Task<Summary> CreateAsync(ModelPlayerInfo playerInfo, IEnumerable<ModelAvatarInfo> avatarInfos, CancellationToken token)
    {
        Dictionary<int, MetadataAvatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync(token).ConfigureAwait(false);
        Dictionary<int, MetadataWeapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false);
        Dictionary<int, FightProperty> idRelicMainPropMap = await metadataService.GetIdToReliquaryMainPropertyMapAsync(token).ConfigureAwait(false);
        Dictionary<int, ReliquaryAffix> idReliquaryAffixMap = await metadataService.GetIdReliquaryAffixMapAsync(token).ConfigureAwait(false);

        List<ReliquaryLevel> reliqueryLevels = await metadataService.GetReliquaryLevelsAsync(token).ConfigureAwait(false);
        List<MetadataReliquary> reliquaries = await metadataService.GetReliquariesAsync(token).ConfigureAwait(false);

        SummaryFactoryImplementation inner = new(idAvatarMap, idWeaponMap, idRelicMainPropMap, idReliquaryAffixMap, reliqueryLevels, reliquaries);
        return inner.Create(playerInfo, avatarInfos);
    }
}