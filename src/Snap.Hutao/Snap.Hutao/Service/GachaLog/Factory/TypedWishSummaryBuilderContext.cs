// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Hutao.Response;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal readonly struct TypedWishSummaryBuilderContext
{
    public readonly ITaskContext TaskContext;
    public readonly HomaGachaLogClient GachaLogClient;
    public readonly string Name;
    public readonly int GuaranteeOrangeThreshold;
    public readonly int GuaranteePurpleThreshold;
    public readonly Func<GachaType, bool> TypeEvaluator;
    public readonly GachaDistributionType DistributionType;

    private static readonly Func<GachaType, bool> IsStandardWish = type => type is GachaType.Standard;
    private static readonly Func<GachaType, bool> IsAvatarEventWish = type => type is GachaType.ActivityAvatar or GachaType.SpecialActivityAvatar;
    private static readonly Func<GachaType, bool> IsWeaponEventWish = type => type is GachaType.ActivityWeapon;
    private static readonly Func<GachaType, bool> IsChronicledWish = type => type is GachaType.ActivityCity;

    public TypedWishSummaryBuilderContext(
        ITaskContext taskContext,
        HomaGachaLogClient gachaLogClient,
        string name,
        int guaranteeOrangeThreshold,
        int guaranteePurpleThreshold,
        Func<GachaType, bool> typeEvaluator,
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

    public static TypedWishSummaryBuilderContext ChronicledWish(ITaskContext taskContext, HomaGachaLogClient gachaLogClient)
    {
        return new(taskContext, gachaLogClient, SH.ServiceGachaLogFactoryChronicledWishName, 90, 10, IsChronicledWish, GachaDistributionType.Chronicled);
    }

    public ValueTask<HutaoResponse<GachaDistribution>> GetGachaDistributionAsync()
    {
        return GachaLogClient.GetGachaDistributionAsync(DistributionType);
    }
}