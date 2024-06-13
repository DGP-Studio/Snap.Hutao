// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("profile_pictures")]
[SuppressMessage("", "SH002")]
internal sealed class UserGameRoleProfilePicture : IMappingFrom<UserGameRoleProfilePicture, PlayerUid, ProfilePicture>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public string Uid { get; set; } = default!;

    public uint ProfilePictureId { get; set; }

    public uint AvatarId { get; set; }

    public uint CostumeId { get; set; }

    public DateTimeOffset LastUpdateTime { get; set; }

    public static UserGameRoleProfilePicture From(PlayerUid uid, ProfilePicture profilePicture)
    {
        return new()
        {
            Uid = uid.ToString(),
            ProfilePictureId = profilePicture.ProfilePictureId,
            AvatarId = profilePicture.AvatarId,
            CostumeId = profilePicture.CostumeId,
            LastUpdateTime = DateTimeOffset.Now,
        };
    }
}
