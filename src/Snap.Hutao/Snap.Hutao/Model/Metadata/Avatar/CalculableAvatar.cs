// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Binding.Gacha.Abstraction;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 可计算角色
/// </summary>
internal class CalculableAvatar : ObservableObject, ICalculableAvatar
{
    private int levelCurrent;
    private int levelTarget;

    /// <summary>
    /// 构造一个新的可计算角色
    /// </summary>
    /// <param name="avatar">角色</param>
    public CalculableAvatar(Avatar avatar)
    {
        AvatarId = avatar.Id;
        LevelMin = 1;
        LevelMax = int.Parse(avatar.Property.Parameters.Last().Level);
        Skills = avatar.SkillDepot.GetCompositeSkillsNoInherents().Select(p => p.ToCalculable()).ToList();
        Name = avatar.Name;
        Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
        Quality = avatar.Quality;
        LevelCurrent = LevelMin;
        LevelTarget = LevelMax;
    }

    /// <inheritdoc/>
    public AvatarId AvatarId { get; }

    /// <inheritdoc/>
    public int LevelMin { get; }

    /// <inheritdoc/>
    public int LevelMax { get; }

    /// <inheritdoc/>
    public IList<ICalculableSkill> Skills { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public Uri Icon { get; }

    /// <inheritdoc/>
    public ItemQuality Quality { get; }

    /// <inheritdoc/>
    public int LevelCurrent { get => levelCurrent; set => SetProperty(ref levelCurrent, value); }

    /// <inheritdoc/>
    public int LevelTarget { get => levelTarget; set => SetProperty(ref levelTarget, value); }
}