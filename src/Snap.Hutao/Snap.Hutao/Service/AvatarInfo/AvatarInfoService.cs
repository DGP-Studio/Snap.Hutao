// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Enka;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 角色信息服务
/// </summary>
[Injection(InjectAs.Transient, typeof(IAvatarInfoService))]
internal class AvatarInfoService : IAvatarInfoService
{
    private readonly AppDbContext appDbContext;
    private readonly ISummaryFactory summaryFactory;
    private readonly IMetadataService metadataService;
    private readonly ILogger<AvatarInfoService> logger;
    private readonly EnkaClient enkaClient;

    /// <summary>
    /// 构造一个新的角色信息服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="summaryFactory">简述工厂</param>
    /// <param name="logger">日志器</param>
    /// <param name="enkaClient">Enka客户端</param>
    public AvatarInfoService(
        AppDbContext appDbContext,
        IMetadataService metadataService,
        ISummaryFactory summaryFactory,
        ILogger<AvatarInfoService> logger,
        EnkaClient enkaClient)
    {
        this.appDbContext = appDbContext;
        this.metadataService = metadataService;
        this.summaryFactory = summaryFactory;
        this.logger = logger;
        this.enkaClient = enkaClient;
    }

    /// <inheritdoc/>
    public async Task<ValueResult<RefreshResult, Summary?>> GetSummaryAsync(PlayerUid uid, RefreshOption refreshOption, CancellationToken token = default)
    {
        if (await metadataService.InitializeAsync(token).ConfigureAwait(false))
        {
            if (HasOption(refreshOption, RefreshOption.RequestFromAPI))
            {
                EnkaResponse? resp = await GetEnkaResponseAsync(uid, token).ConfigureAwait(false);
                if (resp == null)
                {
                    return new(RefreshResult.APIUnavailable, null);
                }

                if (resp.IsValid)
                {
                    IList<Web.Enka.Model.AvatarInfo> list = HasOption(refreshOption, RefreshOption.StoreInDatabase)
                        ? UpdateDbAvatarInfo(uid.Value, resp.AvatarInfoList)
                        : resp.AvatarInfoList;

                    Summary summary = await GetSummaryCoreAsync(resp.PlayerInfo, list).ConfigureAwait(false);
                    return new(RefreshResult.Ok, summary);
                }
                else
                {
                    return new(RefreshResult.ShowcaseNotOpen, null);
                }
            }
            else
            {
                PlayerInfo info = PlayerInfo.CreateEmpty(uid.Value);

                Summary summary = await GetSummaryCoreAsync(info, GetDbAvatarInfos(uid.Value)).ConfigureAwait(false);
                return new(RefreshResult.Ok, summary);
            }
        }
        else
        {
            return new(RefreshResult.MetadataUninitialized, null);
        }
    }

    private static bool HasOption(RefreshOption source, RefreshOption define)
    {
        return (source & define) == define;
    }

    private async Task<Summary> GetSummaryCoreAsync(PlayerInfo info, IEnumerable<Web.Enka.Model.AvatarInfo> avatarInfos)
    {
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();
        Summary summary = await summaryFactory.CreateAsync(info, avatarInfos).ConfigureAwait(false);
        logger.LogInformation(EventIds.AvatarInfoGeneration, "AvatarInfoSummary Generation toke {time} ms.", stopwatch.GetElapsedTime().TotalMilliseconds);

        return summary;
    }

    private async Task<EnkaResponse?> GetEnkaResponseAsync(PlayerUid uid, CancellationToken token = default)
    {
        return await enkaClient.GetForwardDataAsync(uid, token).ConfigureAwait(false)
            ?? await enkaClient.GetDataAsync(uid, token).ConfigureAwait(false);
    }

    private List<Web.Enka.Model.AvatarInfo> UpdateDbAvatarInfo(string uid, IEnumerable<Web.Enka.Model.AvatarInfo> webInfos)
    {
        List<Model.Entity.AvatarInfo> dbInfos = appDbContext.AvatarInfos
            .Where(i => i.Uid == uid)
            .ToList();

        foreach (Web.Enka.Model.AvatarInfo webInfo in webInfos)
        {
            if (webInfo.AvatarId == 10000005 || webInfo.AvatarId == 10000007)
            {
                continue;
            }

            Model.Entity.AvatarInfo? entity = dbInfos.SingleOrDefault(i => i.Info.AvatarId == webInfo.AvatarId);

            if (entity == null)
            {
                entity = Model.Entity.AvatarInfo.Create(uid, webInfo);
                appDbContext.Add(entity);
            }
            else
            {
                entity.Info = webInfo;
                appDbContext.Update(entity);
            }
        }

        appDbContext.SaveChanges();

        return GetDbAvatarInfos(uid);
    }

    private List<Web.Enka.Model.AvatarInfo> GetDbAvatarInfos(string uid)
    {
        return appDbContext.AvatarInfos
            .Where(i => i.Uid == uid)
            .Select(i => i.Info)

            // .AsEnumerable()
            // .OrderByDescending(i => i.AvatarId)
            .ToList();
    }
}