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
