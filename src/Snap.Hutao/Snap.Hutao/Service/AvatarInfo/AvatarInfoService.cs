// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Enka;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using EnkaAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 角色信息服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAvatarInfoService))]
internal sealed partial class AvatarInfoService : IAvatarInfoService
{
    private readonly AvatarInfoDbBulkOperation avatarInfoDbBulkOperation;
    private readonly IAvatarInfoDbService avatarInfoDbService;
    private readonly ILogger<AvatarInfoService> logger;
    private readonly IMetadataService metadataService;
    private readonly ISummaryFactory summaryFactory;
    private readonly EnkaClient enkaClient;

    /// <inheritdoc/>
    public async ValueTask<ValueResult<RefreshResult, Summary?>> GetSummaryAsync(UserAndUid userAndUid, RefreshOption refreshOption, CancellationToken token = default)
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
                        if (resp is null)
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

                        List<EntityAvatarInfo> list = avatarInfoDbBulkOperation.UpdateDbAvatarInfosByShowcase(userAndUid.Uid.Value, resp.AvatarInfoList, token);
                        Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                case RefreshOption.RequestFromHoyolabGameRecord:
                    {
                        List<EntityAvatarInfo> list = await avatarInfoDbBulkOperation.UpdateDbAvatarInfosByGameRecordCharacterAsync(userAndUid, token).ConfigureAwait(false);
                        Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                case RefreshOption.RequestFromHoyolabCalculate:
                    {
                        List<EntityAvatarInfo> list = await avatarInfoDbBulkOperation.UpdateDbAvatarInfosByCalculateAvatarDetailAsync(userAndUid, token).ConfigureAwait(false);
                        Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                default:
                    {
                        List<EntityAvatarInfo> list = avatarInfoDbService.GetAvatarInfoListByUid(userAndUid.Uid.Value);
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

    private async ValueTask<EnkaResponse?> GetEnkaResponseAsync(PlayerUid uid, CancellationToken token = default)
    {
        return await enkaClient.GetForwardDataAsync(uid, token).ConfigureAwait(false)
            ?? await enkaClient.GetDataAsync(uid, token).ConfigureAwait(false);
    }

    private async ValueTask<Summary> GetSummaryCoreAsync(IEnumerable<Model.Entity.AvatarInfo> avatarInfos, CancellationToken token)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            return await summaryFactory.CreateAsync(avatarInfos, token).ConfigureAwait(false);
        }
    }
}