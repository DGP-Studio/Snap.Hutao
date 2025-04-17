// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

internal sealed class SimpleRecord
{
    public SimpleRecord(string uid, ImmutableArray<DetailedCharacter> characters, Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss, string? reservedUserName)
    {
        Uid = uid;
        Identity = "Snap Hutao"; // hardcoded Identity name
        SpiralAbyss = new(spiralAbyss);
        Avatars = characters.SelectAsArray(static a => new SimpleAvatar(a));
        ReservedUserName = reservedUserName;
    }

    public string Uid { get; set; }

    public string Identity { get; set; }

    public string? ReservedUserName { get; set; }

    public SimpleSpiralAbyss SpiralAbyss { get; set; }

    public ImmutableArray<SimpleAvatar> Avatars { get; set; }
}