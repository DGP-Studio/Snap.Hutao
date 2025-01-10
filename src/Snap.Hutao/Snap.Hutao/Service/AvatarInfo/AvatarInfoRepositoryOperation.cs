// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Response;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class AvatarInfoRepositoryOperation
{
    private readonly IAvatarInfoRepository avatarInfoRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    public async ValueTask<ImmutableArray<EntityAvatarInfo>> UpdateDbAvatarInfosAsync(UserAndUid userAndUid, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        string uid = userAndUid.Uid.Value;
        ImmutableArray<EntityAvatarInfo> dbInfos = avatarInfoRepository.GetAvatarInfoImmutableArrayByUid(uid);
        EnsureItemsAvatarIdUnique(dbInfos, uid, out FrozenDictionary<AvatarId, EntityAvatarInfo> dbInfoMap);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            IGameRecordClient gameRecordClient = serviceProvider
                .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                .CreateFor(userAndUid);

            // This is a tricky way to immediately update the avatar info, this behavior
            // can change in the future by miHoYo, so it's not recommended to rely on this.
            await gameRecordClient.GetPlayerInfoAsync(userAndUid, token).ConfigureAwait(false);

            Response<ListWrapper<Character>> listResponse = await gameRecordClient
                .GetCharacterListAsync(userAndUid, token)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(listResponse, serviceProvider, out ListWrapper<Character>? charactersWrapper))
            {
                return avatarInfoRepository.GetAvatarInfoImmutableArrayByUid(uid);
            }

            ImmutableArray<AvatarId> characterIds = charactersWrapper.List.SelectAsArray(info => info.Id);
            Response<ListWrapper<DetailedCharacter>> detailResponse = await gameRecordClient
                .GetCharacterDetailAsync(userAndUid, characterIds, token)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(detailResponse, serviceProvider, out ListWrapper<DetailedCharacter>? detailsWrapper))
            {
                return avatarInfoRepository.GetAvatarInfoImmutableArrayByUid(uid);
            }

            foreach (DetailedCharacter character in detailsWrapper.List)
            {
                if (AvatarIds.IsPlayer(character.Base.Id))
                {
                    continue;
                }

                // We can only obtain new avatar, and we can't lose the avatar we already have.
                // So we don't need to remove any avatar info from the database.
                EntityAvatarInfo? entity = dbInfoMap.GetValueOrDefault(character.Base.Id);
                AddOrUpdateAvatarInfo(entity, uid, appDbContext, character);
            }
        }

        return avatarInfoRepository.GetAvatarInfoImmutableArrayByUid(uid);
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

    private void EnsureItemsAvatarIdUnique(ImmutableArray<EntityAvatarInfo> dbInfos, string uid, out FrozenDictionary<AvatarId, EntityAvatarInfo> dbInfoMap)
    {
        Dictionary<AvatarId, EntityAvatarInfo> infoMap = [];
        foreach (ref readonly EntityAvatarInfo info in dbInfos.AsSpan())
        {
            if (info.Info2 is null || !infoMap.TryAdd(info.Info2.Base.Id, info))
            {
                avatarInfoRepository.RemoveAvatarInfoRangeByUid(uid);
                dbInfoMap = FrozenDictionary<AvatarId, EntityAvatarInfo>.Empty;
                return;
            }
        }

        dbInfoMap = infoMap.ToFrozenDictionary();
    }
}