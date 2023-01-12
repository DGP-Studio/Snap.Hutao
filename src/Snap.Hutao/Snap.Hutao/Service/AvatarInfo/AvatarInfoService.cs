// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Enka;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using EnkaAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using EnkaPlayerInfo = Snap.Hutao.Web.Enka.Model.PlayerInfo;

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 角色信息服务
/// </summary>
[Injection(InjectAs.Scoped, typeof(IAvatarInfoService))]
internal class AvatarInfoService : IAvatarInfoService
{
    private readonly AppDbContext appDbContext;
    private readonly ISummaryFactory summaryFactory;
    private readonly IMetadataService metadataService;
    private readonly ILogger<AvatarInfoService> logger;

    private readonly AvatarInfoDbOperation avatarInfoDbOperation;

    /// <summary>
    /// 构造一个新的角色信息服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="summaryFactory">简述工厂</param>
    /// <param name="logger">日志器</param>
    public AvatarInfoService(
        AppDbContext appDbContext,
        IMetadataService metadataService,
        ISummaryFactory summaryFactory,
        ILogger<AvatarInfoService> logger)
    {
        this.appDbContext = appDbContext;
        this.metadataService = metadataService;
        this.summaryFactory = summaryFactory;
        this.logger = logger;

        avatarInfoDbOperation = new(appDbContext);
    }

    /// <inheritdoc/>
    public async Task<ValueResult<RefreshResult, Summary?>> GetSummaryAsync(UserAndUid userAndUid, RefreshOption refreshOption, CancellationToken token = default)
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            token.ThrowIfCancellationRequested();

            switch (refreshOption)
            {
                case RefreshOption.RequestFromEnkaAPI:
                    {
                        EnkaResponse? resp = await GetEnkaResponseAsync(userAndUid.Uid, token).ConfigureAwait(false);
                        token.ThrowIfCancellationRequested();
                        if (resp == null)
                        {
                            return new(RefreshResult.APIUnavailable, null);
                        }

                        if (!resp.IsValid)
                        {
                            return new(RefreshResult.ShowcaseNotOpen, null);
                        }

                        List<EnkaAvatarInfo> list = avatarInfoDbOperation.UpdateDbAvatarInfos(userAndUid.Uid.Value, resp.AvatarInfoList, token);
                        Summary summary = await GetSummaryCoreAsync(resp.PlayerInfo, list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                case RefreshOption.RequestFromHoyolabGameRecord:
                    {
                        EnkaPlayerInfo info = EnkaPlayerInfo.CreateEmpty(userAndUid.Uid.Value);
                        List<EnkaAvatarInfo> list = await avatarInfoDbOperation.UpdateDbAvatarInfosByGameRecordCharacterAsync(userAndUid, token).ConfigureAwait(false);
                        Summary summary = await GetSummaryCoreAsync(info, list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                case RefreshOption.RequestFromHoyolabCalculate:
                    {
                        EnkaPlayerInfo info = EnkaPlayerInfo.CreateEmpty(userAndUid.Uid.Value);
                        List<EnkaAvatarInfo> list = await avatarInfoDbOperation.UpdateDbAvatarInfosByCalculateAvatarDetailAsync(userAndUid, token).ConfigureAwait(false);
                        Summary summary = await GetSummaryCoreAsync(info, list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                default:
                    {
                        EnkaPlayerInfo info = EnkaPlayerInfo.CreateEmpty(userAndUid.Uid.Value);
                        List<EnkaAvatarInfo> list = avatarInfoDbOperation.GetDbAvatarInfos(userAndUid.Uid.Value);
                        Summary summary = await GetSummaryCoreAsync(info, list, token).ConfigureAwait(false);
                        token.ThrowIfCancellationRequested();
                        return new(RefreshResult.Ok, summary.Avatars.Count == 0 ? null : summary);
                    }
            }
        }
        else
        {
            return new(RefreshResult.MetadataNotInitialized, null);
        }
    }

    private static async Task<EnkaResponse?> GetEnkaResponseAsync(PlayerUid uid, CancellationToken token = default)
    {
        EnkaClient enkaClient = Ioc.Default.GetRequiredService<EnkaClient>();

        return await enkaClient.GetForwardDataAsync(uid, token).ConfigureAwait(false)
            ?? await enkaClient.GetDataAsync(uid, token).ConfigureAwait(false);
    }

    private async Task<Summary> GetSummaryCoreAsync(EnkaPlayerInfo info, IEnumerable<EnkaAvatarInfo> avatarInfos, CancellationToken token)
    {
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();
        Summary summary = await summaryFactory.CreateAsync(info, avatarInfos, token).ConfigureAwait(false);
        logger.LogInformation(EventIds.AvatarInfoGeneration, "AvatarInfoSummary Generation toke {time} ms.", stopwatch.GetElapsedTime().TotalMilliseconds);

        return summary;
    }
}