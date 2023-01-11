// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Service.AvatarInfo.Composer;
using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Enka;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Response;
using CalculateAvatar = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Avatar;
using EnkaAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using EnkaPlayerInfo = Snap.Hutao.Web.Enka.Model.PlayerInfo;
using ModelAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;
using RecordCharacter = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar.Character;
using RecordPlayerInfo = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.PlayerInfo;

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

                        if (resp.IsValid)
                        {
                            IList<EnkaAvatarInfo> list = UpdateDbAvatarInfos(userAndUid.Uid.Value, resp.AvatarInfoList);
                            Summary summary = await GetSummaryCoreAsync(resp.PlayerInfo, list, token).ConfigureAwait(false);
                            token.ThrowIfCancellationRequested();
                            return new(RefreshResult.Ok, summary);
                        }
                        else
                        {
                            return new(RefreshResult.ShowcaseNotOpen, null);
                        }
                    }

                case RefreshOption.RequestFromHoyolabGameRecord:
                    {
                        EnkaPlayerInfo info = EnkaPlayerInfo.CreateEmpty(userAndUid.Uid.Value);
                        IList<EnkaAvatarInfo> list = await UpdateDbAvatarInfosByGameRecordCharacterAsync(userAndUid).ConfigureAwait(false);
                        Summary summary = await GetSummaryCoreAsync(info, list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                case RefreshOption.RequestFromHoyolabCalculate:
                    {
                        EnkaPlayerInfo info = EnkaPlayerInfo.CreateEmpty(userAndUid.Uid.Value);
                        IList<EnkaAvatarInfo> list = await UpdateDbAvatarInfosByCalculateAvatarDetailAsync(userAndUid).ConfigureAwait(false);
                        Summary summary = await GetSummaryCoreAsync(info, list, token).ConfigureAwait(false);
                        return new(RefreshResult.Ok, summary);
                    }

                default:
                    {
                        EnkaPlayerInfo info = EnkaPlayerInfo.CreateEmpty(userAndUid.Uid.Value);
                        Summary summary = await GetSummaryCoreAsync(info, GetDbAvatarInfos(userAndUid.Uid.Value), token).ConfigureAwait(false);
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

    private List<EnkaAvatarInfo> UpdateDbAvatarInfos(string uid, IEnumerable<EnkaAvatarInfo> webInfos)
    {
        List<ModelAvatarInfo> dbInfos = appDbContext.AvatarInfos
            .Where(i => i.Uid == uid)
            .ToList();

        foreach (EnkaAvatarInfo webInfo in webInfos)
        {
            if (AvatarIds.IsPlayer(webInfo.AvatarId))
            {
                continue;
            }

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

        return GetDbAvatarInfos(uid);
    }

    private async Task<List<EnkaAvatarInfo>> UpdateDbAvatarInfosByGameRecordCharacterAsync(UserAndUid userAndUid)
    {
        string uid = userAndUid.Uid.Value;
        List<ModelAvatarInfo> dbInfos = appDbContext.AvatarInfos
            .Where(i => i.Uid == uid)
            .ToList();

        GameRecordClient gameRecordClient = Ioc.Default.GetRequiredService<GameRecordClient>();
        Response<RecordPlayerInfo> playerInfoResponse = await gameRecordClient
            .GetPlayerInfoAsync(userAndUid)
            .ConfigureAwait(false);

        // TODO: We should not refresh if response is not correct here.
        if (playerInfoResponse.IsOk())
        {
            Response<Web.Hoyolab.Takumi.GameRecord.Avatar.CharacterWrapper> charactersResponse = await gameRecordClient
                .GetCharactersAsync(userAndUid, playerInfoResponse.Data)
                .ConfigureAwait(false);

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
        }

        return GetDbAvatarInfos(uid);
    }

    private async Task<List<EnkaAvatarInfo>> UpdateDbAvatarInfosByCalculateAvatarDetailAsync(UserAndUid userAndUid)
    {
        string uid = userAndUid.Uid.Value;
        List<ModelAvatarInfo> dbInfos = appDbContext.AvatarInfos
            .Where(i => i.Uid == uid)
            .ToList();

        CalculateClient calculateClient = Ioc.Default.GetRequiredService<CalculateClient>();
        List<CalculateAvatar> avatars = await calculateClient.GetAvatarsAsync(userAndUid).ConfigureAwait(false);

        CalculateAvatarDetailAvatarInfoComposer composer = Ioc.Default.GetRequiredService<CalculateAvatarDetailAvatarInfoComposer>();

        foreach (CalculateAvatar avatar in avatars)
        {
            if (AvatarIds.IsPlayer(avatar.Id))
            {
                continue;
            }

            Response<AvatarDetail> detailAvatarResponse = await calculateClient.GetAvatarDetailAsync(userAndUid, avatar).ConfigureAwait(false);

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

    private List<EnkaAvatarInfo> GetDbAvatarInfos(string uid)
    {
        try
        {
            return appDbContext.AvatarInfos
                .Where(i => i.Uid == uid)
                .Select(i => i.Info)
                .ToList();
        }
        catch (ObjectDisposedException)
        {
            // appDbContext can be disposed unexpectedly
            return new();
        }
    }
}