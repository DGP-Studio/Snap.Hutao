// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Abstraction.Extension;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class ScoreAccessExtension
{
    public static TBuilder SetScore<TBuilder>(this TBuilder builder, float score)
        where TBuilder : IBuilder, IScoreAccess
    {
        return builder.Configure(b => b.Score = score);
    }
}