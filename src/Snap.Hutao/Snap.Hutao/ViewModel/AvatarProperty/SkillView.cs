// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 天赋
/// </summary>
[HighQuality]
internal sealed class SkillView : NameIconDescription, ICalculableSource<ICalculableSkill>
{
    /// <summary>
    /// 技能属性
    /// </summary>
    public LevelParameters<string, ParameterDescription> Info { get; set; } = default!;

    /// <summary>
    /// 不计算命座的技能等级
    /// </summary>
    public int LevelNumber { get; set; }

    /// <summary>
    /// 不计算命座的技能等级字符串
    /// </summary>
    public string Level { get => $"Lv.{LevelNumber}"; }

    /// <summary>
    /// 技能组Id
    /// </summary>
    internal int GroupId { get; set; }

    /// <inheritdoc/>
    public ICalculableSkill ToCalculable()
    {
        return new CalculableSkill(this);
    }
}
