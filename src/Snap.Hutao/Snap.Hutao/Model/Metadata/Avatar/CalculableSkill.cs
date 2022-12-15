// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 可计算的技能
/// </summary>
internal class CalculableSkill : ObservableObject, ICalculableSkill
{
    private int levelCurrent;
    private int levelTarget;

    /// <summary>
    /// 构造一个新的可计算的技能
    /// </summary>
    /// <param name="skill">技能</param>
    public CalculableSkill(ProudableSkill skill)
    {
        GruopId = skill.GroupId;
        LevelMin = 1;
        LevelMax = 10; // hard coded 10 here
        Name = skill.Name;
        Icon = SkillIconConverter.IconNameToUri(skill.Icon);
        Quality = ItemQuality.QUALITY_NONE;
        LevelCurrent = LevelMin;
        LevelTarget = LevelMax;
    }

    /// <inheritdoc/>
    public int GruopId { get; }

    /// <inheritdoc/>
    public int LevelMin { get; }

    /// <inheritdoc/>
    public int LevelMax { get; }

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