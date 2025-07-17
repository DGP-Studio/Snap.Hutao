// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.HardChallenge;

internal interface IHardChallengeRepository : IRepository<HardChallengeEntry>
{
    void AddHardChallengeEntry(HardChallengeEntry entry);

    FrozenDictionary<uint, HardChallengeEntry> GetHardChallengeMapByUid(string uid);

    void UpdateHardChallengeEntry(HardChallengeEntry entry);
}