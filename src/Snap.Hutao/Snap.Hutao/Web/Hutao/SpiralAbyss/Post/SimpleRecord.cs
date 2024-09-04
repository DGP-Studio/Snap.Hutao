// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

internal sealed class SimpleRecord
{
    public SimpleRecord(string uid, List<DetailedCharacter> characters, Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss, string? reservedUserName)
    {
        Uid = uid;
        Identity = "Snap Hutao"; // hardcoded Identity name
        SpiralAbyss = new(spiralAbyss);
        Avatars = characters.Select(a => new SimpleAvatar(a));
        ReservedUserName = reservedUserName;
    }

    public string Uid { get; set; } = default!;

    public string Identity { get; set; } = default!;

    public string? ReservedUserName { get; set; }

    public SimpleSpiralAbyss SpiralAbyss { get; set; } = default!;

    public IEnumerable<SimpleAvatar> Avatars { get; set; } = default!;
}
