// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class ProfilePicture
{
    public required ProfilePictureId Id { get; init; }

    public required string Icon { get; init; }

    public required string Name { get; init; }

    public required ProfilePictureUnlockType UnlockType { get; init; }

    /// <summary>
    /// <see cref="ProfilePictureUnlockType.Item"/> -> <see cref="MaterialId"/>
    /// <br/>
    /// <see cref="ProfilePictureUnlockType.Avatar"/> -> <see cref="AvatarId"/>
    /// <br/>
    /// <see cref="ProfilePictureUnlockType.Costume"/> -> <see cref="CostumeId"/>
    /// <br/>
    /// <see cref="ProfilePictureUnlockType.ParentQuest"/> -> <see cref="QuestId"/>
    /// </summary>
    public uint UnlockParameter { get; init; }
}
