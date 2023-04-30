// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Service.AvatarInfo.Transformer;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
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
[Injection(InjectAs.Scoped)]
internal sealed class AvatarInfoDbBulkOperation
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的角色信息数据库操作
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AvatarInfoDbBulkOperation(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

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
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<ModelAvatarInfo> dbInfos = appDbContext.AvatarInfos
                .Where(i => i.Uid == uid)
                .ToList();
            EnsureItemsAvatarIdDistinct(ref dbInfos, uid);

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
            return GetDbAvatarInfos(uid);
        }
    }

    /// <summary>
    /// 米游社我的角色方式 更新数据库角色信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public async Task<List<EnkaAvatarInfo>> UpdateDbAvatarInfosByGameRecordCharacterAsync(UserAndUid userAndUid, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        string uid = userAndUid.Uid.Value;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<ModelAvatarInfo> dbInfos = appDbContext.AvatarInfos
                .Where(i => i.Uid == uid)
                .ToList();
            EnsureItemsAvatarIdDistinct(ref dbInfos, uid);

            IGameRecordClient gameRecordClient = serviceProvider.PickRequiredService<IGameRecordClient>(userAndUid.User.IsOversea);
            Response<RecordPlayerInfo> playerInfoResponse = await gameRecordClient
                .GetPlayerInfoAsync(userAndUid, token)
                .ConfigureAwait(false);

            if (playerInfoResponse.IsOk())
            {
                Response<Web.Hoyolab.Takumi.GameRecord.Avatar.CharacterWrapper> charactersResponse = await gameRecordClient
                    .GetCharactersAsync(userAndUid, playerInfoResponse.Data, token)
                    .ConfigureAwait(false);

                token.ThrowIfCancellationRequested();

                if (charactersResponse.IsOk())
                {
                    List<RecordCharacter> characters = charactersResponse.Data.Avatars;

                    GameRecordCharacterAvatarInfoTransformer transformer = serviceProvider.GetRequiredService<GameRecordCharacterAvatarInfoTransformer>();
                    transformer.IdAvatarMap = await serviceProvider
                        .GetRequiredService<IMetadataService>()
                        .GetIdToAvatarMapAsync(token)
                        .ConfigureAwait(false);

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

        return GetDbAvatarInfos(uid);
    }

    /// <summary>
    /// 米游社养成计算方式 更新数据库角色信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public async Task<List<EnkaAvatarInfo>> UpdateDbAvatarInfosByCalculateAvatarDetailAsync(UserAndUid userAndUid, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        string uid = userAndUid.Uid.Value;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<ModelAvatarInfo> dbInfos = appDbContext.AvatarInfos
                .Where(i => i.Uid == uid)
                .ToList();
            EnsureItemsAvatarIdDistinct(ref dbInfos, uid);

            CalculateClient calculateClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
            List<CalculateAvatar> avatars = await calculateClient
                .GetAvatarsAsync(userAndUid, token)
                .ConfigureAwait(false);

            CalculateAvatarDetailAvatarInfoTransformer transformer = scope.ServiceProvider.GetRequiredService<CalculateAvatarDetailAvatarInfoTransformer>();

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

                token.ThrowIfCancellationRequested();

                if (!detailAvatarResponse.IsOk())
                {
                    continue;
                }

                ModelAvatarInfo? entity = dbInfos.SingleOrDefault(i => i.Info.AvatarId == avatar.Id);
                AvatarDetail detailAvatar = detailAvatarResponse.Data;
                AddOrUpdateAvatarInfo(entity, avatar.Id, uid, appDbContext, transformer, detailAvatar);
            }
        }

        return GetDbAvatarInfos(uid);
    }

    /// <summary>
    /// 获取数据库角色信息
    /// </summary>
    /// <param name="uid">Uid</param>
    /// <returns>角色列表</returns>
    public List<EnkaAvatarInfo> GetDbAvatarInfos(string uid)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.AvatarInfos
                .Where(i => i.Uid == uid)
                .Select(i => i.Info)
                .ToList();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddOrUpdateAvatarInfo<TSource>(ModelAvatarInfo? entity, int avatarId, string uid, AppDbContext appDbContext, IAvatarInfoTransformer<TSource> transformer, TSource source)
    {
        if (entity == null)
        {
            EnkaAvatarInfo avatarInfo = new() { AvatarId = avatarId };
            transformer.Transform(ref avatarInfo, source);
            entity = ModelAvatarInfo.Create(uid, avatarInfo);
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
            entity = ModelAvatarInfo.Create(uid, webInfo);
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
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                appDbContext.AvatarInfos.ExecuteDeleteWhere(i => i.Uid == uid);
            }

            dbInfos = new();
        }
    }
}