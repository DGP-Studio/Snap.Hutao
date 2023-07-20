// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.AvatarInfo.Transformer;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Response;
using System.Runtime.CompilerServices;
using CalculateAvatar = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Avatar;
using EnkaAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using ModelAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;
using RecordCharacter = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar.Character;
using RecordPlayerInfo = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.PlayerInfo;

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 角色信息数据库操作
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AvatarInfoDbBulkOperation
{
    private readonly IServiceProvider serviceProvider;
    private readonly IAvatarInfoDbService avatarInfoDbService;

    /// <summary>
    /// 更新数据库角色信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="webInfos">Enka信息</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public List<EnkaAvatarInfo> UpdateDbAvatarInfos(string uid, IEnumerable<EnkaAvatarInfo> webInfos, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        List<ModelAvatarInfo> dbInfos = avatarInfoDbService.GetAvatarInfoListByUid(uid);
        EnsureItemsAvatarIdDistinct(ref dbInfos, uid);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            foreach (EnkaAvatarInfo webInfo in webInfos)
            {
                if (AvatarIds.IsPlayer(webInfo.AvatarId))
                {
                    continue;
                }

                token.ThrowIfCancellationRequested();
                ModelAvatarInfo? entity = dbInfos.SingleOrDefault(i => i.Info.AvatarId == webInfo.AvatarId);
                AddOrUpdateAvatarInfo(entity, uid, appDbContext, webInfo);
            }

            token.ThrowIfCancellationRequested();
            return avatarInfoDbService.GetAvatarInfoInfoListByUid(uid);
        }
    }

    /// <summary>
    /// 米游社我的角色方式 更新数据库角色信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public async ValueTask<List<EnkaAvatarInfo>> UpdateDbAvatarInfosByGameRecordCharacterAsync(UserAndUid userAndUid, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        string uid = userAndUid.Uid.Value;
        List<ModelAvatarInfo> dbInfos = avatarInfoDbService.GetAvatarInfoListByUid(uid);
        EnsureItemsAvatarIdDistinct(ref dbInfos, uid);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            IGameRecordClient gameRecordClient = serviceProvider
                .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                .CreateFor(userAndUid);
            Response<RecordPlayerInfo> playerInfoResponse = await gameRecordClient
                .GetPlayerInfoAsync(userAndUid, token)
                .ConfigureAwait(false);

            if (playerInfoResponse.IsOk())
            {
                Response<CharacterWrapper> charactersResponse = await gameRecordClient
                    .GetCharactersAsync(userAndUid, playerInfoResponse.Data, token)
                    .ConfigureAwait(false);

                token.ThrowIfCancellationRequested();

                if (charactersResponse.IsOk())
                {
                    List<RecordCharacter> characters = charactersResponse.Data.Avatars;

                    GameRecordCharacterAvatarInfoTransformer transformer = serviceProvider
                        .GetRequiredService<GameRecordCharacterAvatarInfoTransformer>();

                    foreach (RecordCharacter character in characters)
                    {
                        if (AvatarIds.IsPlayer(character.Id))
                        {
                            continue;
                        }

                        token.ThrowIfCancellationRequested();
                        ModelAvatarInfo? entity = dbInfos.SingleOrDefault(i => i.Info.AvatarId == character.Id);
                        AddOrUpdateAvatarInfo(entity, character.Id, uid, appDbContext, transformer, character);
                    }
                }
            }
        }

        return avatarInfoDbService.GetAvatarInfoInfoListByUid(uid);
    }

    /// <summary>
    /// 米游社养成计算方式 更新数据库角色信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public async ValueTask<List<EnkaAvatarInfo>> UpdateDbAvatarInfosByCalculateAvatarDetailAsync(UserAndUid userAndUid, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        string uid = userAndUid.Uid.Value;
        List<ModelAvatarInfo> dbInfos = avatarInfoDbService.GetAvatarInfoListByUid(uid);
        EnsureItemsAvatarIdDistinct(ref dbInfos, uid);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            CalculateClient calculateClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
            List<CalculateAvatar> avatars = await calculateClient
                .GetAvatarsAsync(userAndUid, token)
                .ConfigureAwait(false);

            CalculateAvatarDetailAvatarInfoTransformer transformer = scope.ServiceProvider
                .GetRequiredService<CalculateAvatarDetailAvatarInfoTransformer>();

            foreach (CalculateAvatar avatar in avatars)
            {
                if (AvatarIds.IsPlayer(avatar.Id))
                {
                    continue;
                }

                token.ThrowIfCancellationRequested();

                Response<AvatarDetail> detailAvatarResponse = await calculateClient
                    .GetAvatarDetailAsync(userAndUid, avatar, token)
                    .ConfigureAwait(false);

                if (!detailAvatarResponse.IsOk())
                {
                    continue;
                }

                token.ThrowIfCancellationRequested();
                ModelAvatarInfo? entity = dbInfos.SingleOrDefault(i => i.Info.AvatarId == avatar.Id);
                AddOrUpdateAvatarInfo(entity, avatar.Id, uid, appDbContext, transformer, detailAvatarResponse.Data);
            }
        }

        return avatarInfoDbService.GetAvatarInfoInfoListByUid(uid);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddOrUpdateAvatarInfo<TSource>(ModelAvatarInfo? entity, in AvatarId avatarId, string uid, AppDbContext appDbContext, IAvatarInfoTransformer<TSource> transformer, TSource source)
    {
        if (entity == null)
        {
            EnkaAvatarInfo avatarInfo = new() { AvatarId = avatarId };
            transformer.Transform(ref avatarInfo, source);
            entity = ModelAvatarInfo.From(uid, avatarInfo);
            appDbContext.AvatarInfos.AddAndSave(entity);
        }
        else
        {
            EnkaAvatarInfo avatarInfo = entity.Info;
            transformer.Transform(ref avatarInfo, source);
            entity.Info = avatarInfo;
            appDbContext.AvatarInfos.UpdateAndSave(entity);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddOrUpdateAvatarInfo(ModelAvatarInfo? entity, string uid, AppDbContext appDbContext, EnkaAvatarInfo webInfo)
    {
        if (entity == null)
        {
            entity = ModelAvatarInfo.From(uid, webInfo);
            appDbContext.AvatarInfos.AddAndSave(entity);
        }
        else
        {
            entity.Info = webInfo;
            appDbContext.AvatarInfos.UpdateAndSave(entity);
        }
    }

    private void EnsureItemsAvatarIdDistinct(ref List<ModelAvatarInfo> dbInfos, string uid)
    {
        int distinctCount = dbInfos.Select(info => info.Info.AvatarId).ToHashSet().Count;

        // Avatars are actually less than the list told us.
        // This means that there are duplicate items.
        if (distinctCount < dbInfos.Count)
        {
            avatarInfoDbService.DeleteAvatarInfoRangeByUid(uid);
            dbInfos = new();
        }
    }
}