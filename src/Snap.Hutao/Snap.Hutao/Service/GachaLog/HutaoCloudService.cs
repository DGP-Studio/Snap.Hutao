// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 胡桃云服务
/// </summary>
[Injection(InjectAs.Scoped, typeof(IHutaoCloudService))]
internal sealed class HutaoCloudService : IHutaoCloudService
{
    private readonly HomaGachaLogClient homaGachaLogClient;
    private readonly AppDbContext appDbContext;

    /// <summary>
    /// 构造一个新的胡桃云服务
    /// </summary>
    /// <param name="homaGachaLogClient">胡桃祈愿记录客户端</param>
    /// <param name="appDbContext">数据库上下文</param>
    public HutaoCloudService(HomaGachaLogClient homaGachaLogClient, AppDbContext appDbContext)
    {
        this.homaGachaLogClient = homaGachaLogClient;
        this.appDbContext = appDbContext;
    }

    /// <inheritdoc/>
    public Task<Response<List<string>>> GetUidsAsync(CancellationToken token = default)
    {
        return homaGachaLogClient.GetUidsAsync(token);
    }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> UploadGachaItemsAsync(Model.Entity.GachaArchive gachaArchive, CancellationToken token = default)
    {
        string uid = gachaArchive.Uid;
        EndIds? endIds = await GetEndIdsFromCloudAsync(uid, token).ConfigureAwait(false);
        if (endIds != null)
        {
            List<GachaItem> items = new();
            foreach ((GachaConfigType type, long endId) in endIds)
            {
                IEnumerable<GachaItem> part = appDbContext.GachaItems
                    .AsNoTracking()
                    .Where(i => i.ArchiveId == gachaArchive.InnerId)
                    .Where(i => i.QueryType == type)
                    .OrderByDescending(i => i.Id)
                    .Where(i => i.Id > endId)

                    // Keep this to make SQL generates correctly
                    .Select(i => new GachaItem()
                    {
                        GachaType = i.GachaType,
                        QueryType = i.QueryType,
                        ItemId = i.ItemId,
                        Time = i.Time,
                        Id = i.Id,
                    });

                items.AddRange(part);
            }

            return await homaGachaLogClient.UploadGachaItemsAsync(uid, items, token).ConfigureAwait(false);
        }

        return new(false, SH.ServiceGachaLogHutaoCloudEndIdFetchFailed);
    }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, Model.Entity.GachaArchive?>> RetrieveGachaItemsAsync(string uid, CancellationToken token = default)
    {
        Model.Entity.GachaArchive? archive = await appDbContext.GachaArchives
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Uid == uid, token)
            .ConfigureAwait(false);

        EndIds endIds = new();
        foreach (GachaConfigType type in GachaLog.QueryTypes)
        {
            if (archive != null)
            {
                Model.Entity.GachaItem? item = appDbContext.GachaItems
                    .AsNoTracking()
                    .Where(i => i.ArchiveId == archive.InnerId)
                    .Where(i => i.QueryType == type)
                    .OrderBy(i => i.Id)
                    .FirstOrDefault();

                if (item != null)
                {
                    endIds[type] = item.Id;
                }
            }
        }

        Response<List<GachaItem>> resp = await homaGachaLogClient.RetrieveGachaItemsAsync(uid, endIds, token).ConfigureAwait(false);

        if (resp.IsOk())
        {
            if (archive == null)
            {
                archive = Model.Entity.GachaArchive.Create(uid);
                await appDbContext.GachaArchives.AddAndSaveAsync(archive).ConfigureAwait(false);
            }

            List<Model.Entity.GachaItem> gachaItems = resp.Data.SelectList(i => Model.Entity.GachaItem.Create(archive.InnerId, i));
            await appDbContext.GachaItems.AddRangeAndSaveAsync(gachaItems).ConfigureAwait(false);
            return new(true, archive);
        }

        return new(false, null);
    }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> DeleteGachaItemsAsync(string uid, CancellationToken token = default)
    {
        return await homaGachaLogClient.DeleteGachaItemsAsync(uid, token).ConfigureAwait(false);
    }

    private async Task<EndIds?> GetEndIdsFromCloudAsync(string uid, CancellationToken token = default)
    {
        Response<EndIds> resp = await homaGachaLogClient.GetEndIdsAsync(uid, token).ConfigureAwait(false);
        _ = resp.IsOk();
        return resp.Data;
    }
}