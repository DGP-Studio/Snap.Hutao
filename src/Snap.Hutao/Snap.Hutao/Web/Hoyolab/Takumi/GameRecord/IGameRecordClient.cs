// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

internal interface IGameRecordClient
{
    ValueTask<Response<ListWrapper<Character>>> GetCharacterListAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<PlayerInfo>> GetPlayerInfoAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<SpiralAbyss.SpiralAbyss>> GetSpiralAbyssAsync(UserAndUid userAndUid, ScheduleType schedule, CancellationToken token = default);

    ValueTask<Response<RoleCombat.RoleCombat>> GetRoleCombatAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<HardChallenge.HardChallenge>> GetHardChallengeAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<HardChallenge.HardChallengePopularity>> GetHardChallengePopularityAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<ListWrapper<DetailedCharacter>>> GetCharacterDetailAsync(UserAndUid userAndUid, ImmutableArray<AvatarId> characterIds, CancellationToken token = default);
}