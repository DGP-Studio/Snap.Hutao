// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 天赋
/// </summary>
public class Skill : NameIconDescription, ICalculableSource<ICalculableSkill>
{
    /// <summary>
    /// 技能属性
    /// </summary>
    public LevelParam<string, ParameterInfo> Info { get; set; } = default!;

    /// <summary>
    /// 技能组Id
    /// </summary>
    internal int GroupId { get; set; }

    /// <summary>
    /// 技能等级，仅用于养成计算
    /// </summary>
    internal int LevelNumber { get; set; }

    /// <inheritdoc/>
    public ICalculableSkill ToCalculable()
    {
        return new CalculableSkill(this);
    }
}
