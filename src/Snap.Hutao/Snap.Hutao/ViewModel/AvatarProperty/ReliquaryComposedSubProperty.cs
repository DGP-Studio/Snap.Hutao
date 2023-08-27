// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal class ReliquaryComposedSubProperty : ReliquarySubProperty
{
    public ReliquaryComposedSubProperty(FightProperty type, string value, float score)
        : base(type, value, score)
    {
        Type = type;
    }

    /// <summary>
    /// 强化次数
    /// </summary>
    public uint EnhancedCount { get; set; }

    internal FightProperty Type { get; }
}