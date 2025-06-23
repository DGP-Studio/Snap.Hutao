// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Wiki;

internal sealed class LinkMetadataContext
{
    public ImmutableDictionary<HyperLinkNameId, HyperLinkName> IdNameMap { get; init; } = default!;

    public ImmutableArray<ProudSkill> Skills { get; init; }

    public ImmutableArray<ProudSkill> Inherents { get; init; }

    public bool TryGetNameAndDescription(MiHoYoSyntaxLinkKind kind, uint id, out string name, out string description)
    {
        name = default!;
        description = default!;

        switch (kind)
        {
            case MiHoYoSyntaxLinkKind.Name:
                HyperLinkName hyperLinkName = IdNameMap[id];
                name = hyperLinkName.Name;
                description = hyperLinkName.Description;
                break;
            case MiHoYoSyntaxLinkKind.Inherent:
                ProudSkill inherent = Inherents.Single(s => s.Id == id);
                name = inherent.Name;
                description = inherent.Description;
                break;
            case MiHoYoSyntaxLinkKind.Skill:
                ProudSkill skill = Skills.Single(s => s.Id == id);
                name = skill.Name;
                description = skill.Description;
                break;
            default:
                return false;
        }

        return true;
    }
}