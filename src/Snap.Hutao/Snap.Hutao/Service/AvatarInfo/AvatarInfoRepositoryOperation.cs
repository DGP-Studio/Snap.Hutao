// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Response;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class AvatarInfoRepositoryOperation
{
    private readonly IAvatarInfoRepository avatarInfoRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    public async ValueTask<List<EntityAvatarInfo>> UpdateDbAvatarInfosAsync(UserAndUid userAndUid, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        string uid = userAndUid.Uid.Value;
        List<EntityAvatarInfo> dbInfos = avatarInfoRepository.GetAvatarInfoListByUid(uid);
        EnsureItemsAvatarIdUnique(ref dbInfos, uid, out Dictionary<AvatarId, EntityAvatarInfo> dbInfoMap);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            IGameRecordClient gameRecordClient = serviceProvider
                .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                .CreateFor(userAndUid);

            // manual refresh avatars
            await gameRecordClient.GetPlayerInfoAsync(userAndUid, token).ConfigureAwait(false);

            Response<ListWrapper<Character>> listResponse = await gameRecordClient
                .GetCharacterListAsync(userAndUid, token)
                .ConfigureAwait(false);

            if (!listResponse.IsOk())
            {
                return avatarInfoRepository.GetAvatarInfoListByUid(uid);
            }

            List<AvatarId> characterIds = listResponse.Data.List.SelectList(info => info.Id);
            Response<ListWrapper<DetailedCharacter>> detailResponse = await gameRecordClient
                .GetCharacterDetailAsync(userAndUid, characterIds, token)
                .ConfigureAwait(false);

            if (!detailResponse.IsOk())
            {
                return avatarInfoRepository.GetAvatarInfoListByUid(uid);
            }

            foreach (DetailedCharacter character in detailResponse.Data.List)
            {
                if (AvatarIds.IsPlayer(character.Base.Id))
                {
                    continue;
                }

                EntityAvatarInfo? entity = dbInfoMap.GetValueOrDefault(character.Base.Id);
                AddOrUpdateAvatarInfo(entity, uid, appDbContext, character);
            }
        }

        return avatarInfoRepository.GetAvatarInfoListByUid(uid);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddOrUpdateAvatarInfo(EntityAvatarInfo? entity, string uid, AppDbContext appDbContext, DetailedCharacter info)
    {
        if (entity is null)
        {
            entity = EntityAvatarInfo.From(uid, info);
        }
        else
        {
            entity.Info2 = info;
        }

        entity.RefreshTime = DateTimeOffset.UtcNow;
        appDbContext.AvatarInfos.UpdateAndSave(entity);
    }

    private void EnsureItemsAvatarIdUnique(ref List<EntityAvatarInfo> dbInfos, string uid, out Dictionary<AvatarId, EntityAvatarInfo> dbInfoMap)
    {
        dbInfoMap = [];
        foreach (ref readonly EntityAvatarInfo info in CollectionsMarshal.AsSpan(dbInfos))
        {
            if (info.Info2 is null || !dbInfoMap.TryAdd(info.Info2.Base.Id, info))
            {
                avatarInfoRepository.RemoveAvatarInfoRangeByUid(uid);
                dbInfoMap.Clear();
                dbInfos.Clear();
                return;
            }
        }
    }
}