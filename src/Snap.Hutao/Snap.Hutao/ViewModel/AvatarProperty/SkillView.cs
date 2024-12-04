// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed class SkillView : NameIconDescription, ITypedCalculableSource<ICalculableSkill, SkillType>
{
    public LevelParameters<string, ParameterDescription> Info { get; set; } = default!;

    public uint LevelNumber { get; set; }

    public string Level { get; set; } = default!;

    internal SkillGroupId GroupId { get; set; }

    public ICalculableSkill ToCalculable(SkillType type)
    {
        return CalculableSkill.From(this, type);
    }
}
