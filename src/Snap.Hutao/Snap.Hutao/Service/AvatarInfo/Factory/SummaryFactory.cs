// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Service.Metadata;
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
        SummaryMetadataContext metadataContext = new()
        {
            IdAvatarMap = await metadataService.GetIdToAvatarMapAsync(token).ConfigureAwait(false),
            IdWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false),
            IdRelicMainPropMap = await metadataService.GetIdToReliquaryMainPropertyMapAsync(token).ConfigureAwait(false),
            IdReliquaryAffixMap = await metadataService.GetIdReliquaryAffixMapAsync(token).ConfigureAwait(false),
            ReliqueryLevels = await metadataService.GetReliquaryLevelsAsync(token).ConfigureAwait(false),
            Reliquaries = await metadataService.GetReliquariesAsync(token).ConfigureAwait(false),
        };

        SummaryFactoryImplementation inner = new(metadataContext);
        return inner.Create(playerInfo, avatarInfos);
    }
}