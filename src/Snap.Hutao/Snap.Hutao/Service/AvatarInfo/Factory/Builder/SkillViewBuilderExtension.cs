// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class SkillViewBuilderExtension
{
    public static TBuilder SetDescription<TBuilder>(this TBuilder builder, string description)
        where TBuilder : class, ISkillViewBuilder
    {
        return builder.SetDescription<TBuilder, SkillView>(description);
    }

    public static TBuilder SetGroupId<TBuilder>(this TBuilder builder, SkillGroupId groupId)
        where TBuilder : ISkillViewBuilder
    {
        builder.View.GroupId = groupId;
        return builder;
    }

    public static TBuilder SetIcon<TBuilder>(this TBuilder builder, Uri icon)
        where TBuilder : class, ISkillViewBuilder
    {
        return builder.SetIcon<TBuilder, SkillView>(icon);
    }

    public static TBuilder SetInfo<TBuilder>(this TBuilder builder, LevelParameters<string, ParameterDescription> info)
        where TBuilder : ISkillViewBuilder
    {
        builder.View.Info = info;
        return builder;
    }

    public static TBuilder SetLevel<TBuilder>(this TBuilder builder, string level)
        where TBuilder : ISkillViewBuilder
    {
        builder.View.Level = level;
        return builder;
    }

    public static TBuilder SetLevelNumber<TBuilder>(this TBuilder builder, uint levelNumber)
        where TBuilder : ISkillViewBuilder
    {
        builder.View.LevelNumber = levelNumber;
        return builder;
    }

    public static TBuilder SetName<TBuilder>(this TBuilder builder, string name)
        where TBuilder : class, ISkillViewBuilder
    {
        return builder.SetName<TBuilder, SkillView>(name);
    }
}