// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录胡桃云服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IGachaLogHutaoCloudService))]
internal sealed partial class GachaLogHutaoCloudService : IGachaLogHutaoCloudService
{
    private readonly IMetadataService metadataService;
    private readonly HomaGachaLogClient homaGachaLogClient;
    private readonly IGachaLogDbService gachaLogDbService;

    /// <inheritdoc/>
    public ValueTask<HutaoResponse<List<GachaEntry>>> GetGachaEntriesAsync(CancellationToken token = default)
    {
        return homaGachaLogClient.GetGachaEntriesAsync(token);
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> UploadGachaItemsAsync(GachaArchive gachaArchive, CancellationToken token = default)
    {
        string uid = gachaArchive.Uid;
        if (await GetEndIdsFromCloudAsync(uid, token).ConfigureAwait(false) is { } endIds)
        {
            List<Web.Hutao.GachaLog.GachaItem> items = new();
            foreach ((GachaConfigType type, long endId) in endIds)
            {
                List<Web.Hutao.GachaLog.GachaItem> part = await gachaLogDbService
                    .GetHutaoGachaItemListAsync(gachaArchive.InnerId, type, endId)
                    .ConfigureAwait(false);
                items.AddRange(part);
            }

            return await homaGachaLogClient.UploadGachaItemsAsync(uid, items, token).ConfigureAwait(false);
        }

        return new(false, SH.ServiceGachaLogHutaoCloudEndIdFetchFailed);
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, Guid>> RetrieveGachaItemsAsync(string uid, CancellationToken token = default)
    {
        GachaArchive? archive = await gachaLogDbService
                .GetGachaArchiveByUidAsync(uid, token)
                .ConfigureAwait(false);

        EndIds endIds = await CreateEndIdsAsync(archive, token).ConfigureAwait(false);
        Response<List<Web.Hutao.GachaLog.GachaItem>> resp = await homaGachaLogClient.RetrieveGachaItemsAsync(uid, endIds, token).ConfigureAwait(false);

        if (!resp.IsOk())
        {
            return new(false, default);
        }

        if (archive is null)
        {
            archive = GachaArchive.From(uid);
            await gachaLogDbService.AddGachaArchiveAsync(archive).ConfigureAwait(false);
        }

        List<Model.Entity.GachaItem> gachaItems = resp.Data.SelectList(i => Model.Entity.GachaItem.From(archive.InnerId, i));
        await gachaLogDbService.AddGachaItemsAsync(gachaItems).ConfigureAwait(false);
        return new(true, archive.InnerId);
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> DeleteGachaItemsAsync(string uid, CancellationToken token = default)
    {
        return await homaGachaLogClient.DeleteGachaItemsAsync(uid, token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, HutaoStatistics>> GetCurrentEventStatisticsAsync(CancellationToken token = default)
    {
        Response<GachaEventStatistics> response = await homaGachaLogClient.GetGachaEventStatisticsAsync(token).ConfigureAwait(false);
        if (response.IsOk())
        {
            if (await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                Dictionary<AvatarId, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync(token).ConfigureAwait(false);
                Dictionary<WeaponId, Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false);
                List<GachaEvent> gachaEvents = await metadataService.GetGachaEventsAsync(token).ConfigureAwait(false);
                HutaoStatisticsFactoryMetadataContext context = new(idAvatarMap, idWeaponMap, gachaEvents);

                GachaEventStatistics raw = response.Data;
                try
                {
                    HutaoStatisticsFactory factory = new(context);
                    HutaoStatistics statistics = factory.Create(raw);
                    return new(true, statistics);
                }
                catch
                {
                    // 元数据未能即时更新导致异常？
                    return new(false, default!);
                }
            }
        }

        return new(false, default!);
    }

    private async ValueTask<EndIds?> GetEndIdsFromCloudAsync(string uid, CancellationToken token = default)
    {
        Response<EndIds> resp = await homaGachaLogClient.GetEndIdsAsync(uid, token).ConfigureAwait(false);
        return resp.IsOk() ? resp.Data : default;
    }

    private async ValueTask<EndIds> CreateEndIdsAsync(GachaArchive? archive, CancellationToken token)
    {
        EndIds endIds = new();
        foreach (GachaConfigType type in GachaLog.QueryTypes)
        {
            if (archive is not null)
            {
                endIds[type] = await gachaLogDbService
                    .GetOldestGachaItemIdByArchiveIdAndQueryTypeAsync(archive.InnerId, type, token)
                    .ConfigureAwait(false);
            }
        }

        return endIds;
    }
}