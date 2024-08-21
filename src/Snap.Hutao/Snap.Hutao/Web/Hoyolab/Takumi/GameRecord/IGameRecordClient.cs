// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 游戏记录提供器
/// </summary>
internal interface IGameRecordClient
{
    ValueTask<Response<ListWrapper<Character>>> GetCharacterListAsync(UserAndUid userAndUid, PlayerInfo playerInfo, CancellationToken token = default);

    ValueTask<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<PlayerInfo>> GetPlayerInfoAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<SpiralAbyss.SpiralAbyss>> GetSpiralAbyssAsync(UserAndUid userAndUid, ScheduleType schedule, CancellationToken token = default);

    ValueTask<Response<RoleCombat.RoleCombat>> GetRoleCombatAsync(UserAndUid userAndUid, CancellationToken token = default);
}