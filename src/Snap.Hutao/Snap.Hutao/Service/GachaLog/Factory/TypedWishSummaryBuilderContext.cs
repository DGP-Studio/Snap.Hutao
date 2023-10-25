// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal readonly struct TypedWishSummaryBuilderContext
{
    public readonly ITaskContext TaskContext;
    public readonly HomaGachaLogClient GachaLogClient;
    public readonly string Name;
    public readonly int GuaranteeOrangeThreshold;
    public readonly int GuaranteePurpleThreshold;
    public readonly Func<GachaConfigType, bool> TypeEvaluator;
    public readonly GachaDistributionType DistributionType;

    private static readonly Func<GachaConfigType, bool> IsStandardWish = type => type is GachaConfigType.StandardWish;
    private static readonly Func<GachaConfigType, bool> IsAvatarEventWish = type => type is GachaConfigType.AvatarEventWish or GachaConfigType.AvatarEventWish2;
    private static readonly Func<GachaConfigType, bool> IsWeaponEventWish = type => type is GachaConfigType.WeaponEventWish;

    public TypedWishSummaryBuilderContext(
        ITaskContext taskContext,
        HomaGachaLogClient gachaLogClient,
        string name,
        int guaranteeOrangeThreshold,
        int guaranteePurpleThreshold,
        Func<GachaConfigType, bool> typeEvaluator,
        GachaDistributionType distributionType)
    {
        TaskContext = taskContext;
        GachaLogClient = gachaLogClient;
        Name = name;
        GuaranteeOrangeThreshold = guaranteeOrangeThreshold;
        GuaranteePurpleThreshold = guaranteePurpleThreshold;
        TypeEvaluator = typeEvaluator;
        DistributionType = distributionType;
    }

    public static TypedWishSummaryBuilderContext StandardWish(ITaskContext taskContext, HomaGachaLogClient gachaLogClient)
    {
        return new(taskContext, gachaLogClient, SH.ServiceGachaLogFactoryPermanentWishName, 90, 10, IsStandardWish, GachaDistributionType.Standard);
    }

    public static TypedWishSummaryBuilderContext AvatarEventWish(ITaskContext taskContext, HomaGachaLogClient gachaLogClient)
    {
        return new(taskContext, gachaLogClient, SH.ServiceGachaLogFactoryAvatarWishName, 90, 10, IsAvatarEventWish, GachaDistributionType.AvatarEvent);
    }

    public static TypedWishSummaryBuilderContext WeaponEventWish(ITaskContext taskContext, HomaGachaLogClient gachaLogClient)
    {
        return new(taskContext, gachaLogClient, SH.ServiceGachaLogFactoryWeaponWishName, 80, 10, IsWeaponEventWish, GachaDistributionType.WeaponEvent);
    }

    public ValueTask<HutaoResponse<GachaDistribution>> GetGachaDistributionAsync()
    {
        return GachaLogClient.GetGachaDistributionAsync(DistributionType);
    }
}