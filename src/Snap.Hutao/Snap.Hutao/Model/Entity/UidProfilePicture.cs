// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("uid_profile_pictures")]
internal sealed class UidProfilePicture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public string Uid { get; set; } = default!;

    public uint ProfilePictureId { get; set; }

    public uint AvatarId { get; set; }

    public uint CostumeId { get; set; }

    public DateTimeOffset RefreshTime { get; set; }

    public static UidProfilePicture From(PlayerUid uid, ProfilePicture profilePicture)
    {
        return new()
        {
            Uid = uid.ToString(),
            ProfilePictureId = profilePicture.Id,
            AvatarId = profilePicture.AvatarId,
            CostumeId = profilePicture.CostumeId,
            RefreshTime = DateTimeOffset.Now,
        };
    }
}
