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
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAvatarInfoService))]
internal sealed partial class AvatarInfoService : IAvatarInfoService
{
    private readonly AvatarInfoRepositoryOperation avatarInfoDbBulkOperation;
    private readonly IAvatarInfoRepository avatarInfoRepository;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<AvatarInfoService> logger;
    private readonly IMetadataService metadataService;
    private readonly ISummaryFactory summaryFactory;

    public async ValueTask<ValueResult<RefreshResultKind, Summary?>> GetSummaryAsync(UserAndUid userAndUid, RefreshOption refreshOption, CancellationToken token = default)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return new(RefreshResultKind.MetadataNotInitialized, null);
        }

        switch (refreshOption)
        {
            case RefreshOption.RequestFromHoyolabGameRecord:
                {
                    List<EntityAvatarInfo> list = await avatarInfoDbBulkOperation.UpdateDbAvatarInfosAsync(userAndUid, token).ConfigureAwait(false);
                    Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
                    return new(RefreshResultKind.Ok, summary);
                }

            default:
                {
                    List<EntityAvatarInfo> list = avatarInfoRepository.GetAvatarInfoListByUid(userAndUid.Uid.Value);
                    Summary summary = await GetSummaryCoreAsync(list, token).ConfigureAwait(false);
                    return new(RefreshResultKind.Ok, summary.Avatars.Count == 0 ? null : summary);
                }
        }
    }

    private async ValueTask<EnkaResponse?> GetEnkaResponseAsync(PlayerUid uid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            EnkaClient enkaClient = scope.ServiceProvider.GetRequiredService<EnkaClient>();

            return await enkaClient.GetForwardDataAsync(uid, token).ConfigureAwait(false)
                ?? await enkaClient.GetDataAsync(uid, token).ConfigureAwait(false);
        }
    }

    private async ValueTask<Summary> GetSummaryCoreAsync(IEnumerable<EntityAvatarInfo> avatarInfos, CancellationToken token)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            return await summaryFactory.CreateAsync(avatarInfos, token).ConfigureAwait(false);
        }
    }
}