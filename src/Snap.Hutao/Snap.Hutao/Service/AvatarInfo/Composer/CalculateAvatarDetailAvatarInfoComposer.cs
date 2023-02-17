// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Service.AvatarInfo.Composer;

/// <summary>
/// 计数器角色详情转角色信息
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient)]
internal sealed class CalculateAvatarDetailAvatarInfoComposer : IAvatarInfoComposer<AvatarDetail>
{
    /// <inheritdoc/>
    public ValueTask<Web.Enka.Model.AvatarInfo> ComposeAsync(Web.Enka.Model.AvatarInfo avatarInfo, AvatarDetail source)
    {
        // update skills
        avatarInfo.SkillLevelMap = source.SkillList.ToDictionary(s => s.Id.ToString(), s => s.LevelCurrent);
        return ValueTask.FromResult(avatarInfo);
    }
}
