// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.ViewModel.User;
using System.Collections.Immutable;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAvatarInfoService))]
internal sealed partial class AvatarInfoService : IAvatarInfoService
{
    private readonly AvatarInfoRepositoryOperation avatarInfoDbBulkOperation;
    private readonly IAvatarInfoRepository avatarInfoRepository;
    private readonly ILogger<AvatarInfoService> logger;
    private readonly ISummaryFactory summaryFactory;

    public async ValueTask<ValueResult<RefreshResultKind, Summary?>> GetSummaryAsync(SummaryFactoryMetadataContext context, UserAndUid userAndUid, RefreshOptionKind refreshOptionKind, CancellationToken token = default)
    {
        switch (refreshOptionKind)
        {
            case RefreshOptionKind.RequestFromHoyolabGameRecord:
                {
                    ImmutableArray<EntityAvatarInfo> list = await avatarInfoDbBulkOperation.UpdateDbAvatarInfosAsync(userAndUid, token).ConfigureAwait(false);
                    Summary summary = await GetSummaryCoreAsync(context, list, token).ConfigureAwait(false);
                    return new(RefreshResultKind.Ok, summary);
                }

            default:
                {
                    ImmutableArray<EntityAvatarInfo> list = avatarInfoRepository.GetAvatarInfoImmutableArrayByUid(userAndUid.Uid.Value);
                    Summary summary = await GetSummaryCoreAsync(context, list, token).ConfigureAwait(false);
                    return new(RefreshResultKind.Ok, summary.Avatars.Count == 0 ? null : summary);
                }
        }
    }

    private async ValueTask<Summary> GetSummaryCoreAsync(SummaryFactoryMetadataContext context, IEnumerable<EntityAvatarInfo> avatarInfos, CancellationToken token)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            return await summaryFactory.CreateAsync(context, avatarInfos, token).ConfigureAwait(false);
        }
    }
}