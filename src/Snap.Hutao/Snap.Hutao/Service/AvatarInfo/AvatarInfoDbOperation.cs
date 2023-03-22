// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Service.AvatarInfo.Composer;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Response;
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
internal sealed class AvatarInfoDbOperation
{
    private readonly AppDbContext appDbContext;

    /// <summary>
    /// 构造一个新的角色信息数据库操作
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    public AvatarInfoDbOperation(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
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

        token.ThrowIfCancellationRequested();
        return GetDbAvatarInfos(uid);
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
        List<ModelAvatarInfo> dbInfos = appDbContext.AvatarInfos
            .Where(i => i.Uid == uid)
            .ToList();
        EnsureItemsAvatarIdDistinct(ref dbInfos, uid);

        Response<RecordPlayerInfo> playerInfoResponse;
        Response<Web.Hoyolab.Takumi.GameRecord.Avatar.CharacterWrapper> charactersResponse;

        if (userAndUid.Uid.Region == "cn_gf01" || userAndUid.Uid.Region == "cn_qd01")
        {
            GameRecordClient gameRecordClient = Ioc.Default.GetRequiredService<GameRecordClient>();
            playerInfoResponse = await gameRecordClient
               .GetPlayerInfoAsync(userAndUid, token)
               .ConfigureAwait(false);

            if (!playerInfoResponse.IsOk())
            {
                return GetDbAvatarInfos(uid);
            }

            charactersResponse = await gameRecordClient
                    .GetCharactersAsync(userAndUid, playerInfoResponse.Data, token)
                    .ConfigureAwait(false);
        }
        else
        {
            GameRecordClientOs gameRecordClientOs = Ioc.Default.GetRequiredService<GameRecordClientOs>();
            playerInfoResponse = await gameRecordClientOs
               .GetPlayerInfoAsync(userAndUid, token)
               .ConfigureAwait(false);

            if (!playerInfoResponse.IsOk())
            {
                return GetDbAvatarInfos(uid);
            }

            charactersResponse = await gameRecordClientOs
                    .GetCharactersAsync(userAndUid, playerInfoResponse.Data, token)
                    .ConfigureAwait(false);
        }

        token.ThrowIfCancellationRequested();

        if (charactersResponse.IsOk())
        {
            List<RecordCharacter> characters = charactersResponse.Data.Avatars;

            GameRecordCharacterAvatarInfoComposer composer = Ioc.Default.GetRequiredService<GameRecordCharacterAvatarInfoComposer>();

            foreach (RecordCharacter character in characters)
            {
                if (AvatarIds.IsPlayer(character.Id))
                {
                    continue;
                }

                token.ThrowIfCancellationRequested();

                ModelAvatarInfo? entity = dbInfos.SingleOrDefault(i => i.Info.AvatarId == character.Id);

                if (entity == null)
                {
                    EnkaAvatarInfo avatarInfo = new() { AvatarId = character.Id };
                    avatarInfo = await composer.ComposeAsync(avatarInfo, character).ConfigureAwait(false);
                    entity = ModelAvatarInfo.Create(uid, avatarInfo);
                    appDbContext.AvatarInfos.AddAndSave(entity);
                }
                else
                {
                    entity.Info = await composer.ComposeAsync(entity.Info, character).ConfigureAwait(false);
                    appDbContext.AvatarInfos.UpdateAndSave(entity);
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
        List<ModelAvatarInfo> dbInfos = appDbContext.AvatarInfos
            .Where(i => i.Uid == uid)
            .ToList();
        EnsureItemsAvatarIdDistinct(ref dbInfos, uid);

        CalculateClient calculateClient = Ioc.Default.GetRequiredService<CalculateClient>();
        List<CalculateAvatar> avatars = await calculateClient.GetAvatarsAsync(userAndUid, token).ConfigureAwait(false);

        CalculateAvatarDetailAvatarInfoComposer composer = Ioc.Default.GetRequiredService<CalculateAvatarDetailAvatarInfoComposer>();

        foreach (CalculateAvatar avatar in avatars)
        {
            if (AvatarIds.IsPlayer(avatar.Id))
            {
                continue;
            }

            token.ThrowIfCancellationRequested();

            Response<AvatarDetail> detailAvatarResponse = await calculateClient.GetAvatarDetailAsync(userAndUid, avatar, token).ConfigureAwait(false);

            token.ThrowIfCancellationRequested();

            if (!detailAvatarResponse.IsOk())
            {
                continue;
            }

            ModelAvatarInfo? entity = dbInfos.SingleOrDefault(i => i.Info.AvatarId == avatar.Id);
            AvatarDetail detailAvatar = detailAvatarResponse.Data;

            if (entity == null)
            {
                EnkaAvatarInfo avatarInfo = new() { AvatarId = avatar.Id };
                avatarInfo = await composer.ComposeAsync(avatarInfo, detailAvatar).ConfigureAwait(false);
                entity = ModelAvatarInfo.Create(uid, avatarInfo);
                appDbContext.AvatarInfos.AddAndSave(entity);
            }
            else
            {
                entity.Info = await composer.ComposeAsync(entity.Info, detailAvatar).ConfigureAwait(false);
                appDbContext.AvatarInfos.UpdateAndSave(entity);
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
        return appDbContext.AvatarInfos
            .Where(i => i.Uid == uid)
            .Select(i => i.Info)
            .ToList();
    }

    private void EnsureItemsAvatarIdDistinct(ref List<ModelAvatarInfo> dbInfos, string uid)
    {
        int distinctCount = dbInfos.Select(info => info.Info.AvatarId).ToHashSet().Count;

        // Avatars are actually less than the list told us.
        if (distinctCount < dbInfos.Count)
        {
            appDbContext.AvatarInfos.ExecuteDeleteWhere(i => i.Uid == uid);
            dbInfos = new();
        }
    }
}