// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Enka;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using EnkaAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 角色信息服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped, typeof(IAvatarInfoService))]
internal sealed class AvatarInfoService : IAvatarInfoService
{
    private readonly AvatarInfoDbBulkOperation avatarInfoDbBulkOperation;
    private readonly ISummaryFactory summaryFactory;
    private readonly IMetadataService metadataService;
    private readonly ILogger<AvatarInfoService> logger;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的角色信息服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="serviceProvider">服务提供器</param>
    public AvatarInfoService(IServiceProvider serviceProvider)
    {
        avatarInfoDbBulkOperation = serviceProvider.GetRequiredService<AvatarInfoDbBulkOperation>();
        metadataService = serviceProvider.GetRequiredService<IMetadataService>();
        summaryFactory = serviceProvider.GetRequiredService<ISummaryFactory>();
        logger = serviceProvider.GetRequiredService<ILogger<AvatarInfoService>>();

        this.serviceProvider = serviceProvider;
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
                            return new(RefreshResult.APIUnavailable, default);
                        }

                        if (!string.IsNullOrEmpty(resp.Message))
                        {
                            return new(RefreshResult.StatusCodeNotSucceed, new Summary { Message = resp.Message });
                        }

                        if (!resp.IsValid)
                        {
                            return new(RefreshResult.ShowcaseNotOpen, default);
                        }

                        List<EnkaAvatarInfo> list = avatarInfoDbBulkOperation.UpdateDbAvatarInfos(userAndUid.Uid.Value, resp.AvatarInfoList, token);
                        Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                case RefreshOption.RequestFromHoyolabGameRecord:
                    {
                        List<EnkaAvatarInfo> list = await avatarInfoDbBulkOperation.UpdateDbAvatarInfosByGameRecordCharacterAsync(userAndUid, token).ConfigureAwait(false);
                        Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                case RefreshOption.RequestFromHoyolabCalculate:
                    {
                        List<EnkaAvatarInfo> list = await avatarInfoDbBulkOperation.UpdateDbAvatarInfosByCalculateAvatarDetailAsync(userAndUid, token).ConfigureAwait(false);
                        Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                default:
                    {
                        List<EnkaAvatarInfo> list = avatarInfoDbBulkOperation.GetDbAvatarInfos(userAndUid.Uid.Value);
                        Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
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

    private async Task<EnkaResponse?> GetEnkaResponseAsync(PlayerUid uid, CancellationToken token = default)
    {
        EnkaClient enkaClient = serviceProvider.GetRequiredService<EnkaClient>();

        return await enkaClient.GetForwardDataAsync(uid, token).ConfigureAwait(false)
            ?? await enkaClient.GetDataAsync(uid, token).ConfigureAwait(false);
    }

    private async Task<Summary> GetSummaryCoreAsync(IEnumerable<EnkaAvatarInfo> avatarInfos, CancellationToken token)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            return await summaryFactory.CreateAsync(avatarInfos, token).ConfigureAwait(false);
        }
    }
}