// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 技能信息的接口实现
/// </summary>
internal sealed partial class ProudableSkill : ITypedCalculableSource<ICalculableSkill, SkillType>
{
    public static uint GetMaxLevel()
    {
        return 10U;
    }

    /// <inheritdoc/>
    public ICalculableSkill ToCalculable(SkillType type)
    {
        return CalculableSkill.From(this, type);
    }
}