// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 技能信息的接口实现
/// </summary>
internal sealed partial class ProudableSkill : ICalculableSource<ICalculableSkill>
{
    /// <inheritdoc/>
    public ICalculableSkill ToCalculable()
    {
        return CalculableSkill.From(this);
    }
}