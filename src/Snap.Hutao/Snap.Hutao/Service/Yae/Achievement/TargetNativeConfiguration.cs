// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Yae.Achievement;

internal sealed class TargetNativeConfiguration
{
    public required uint StoreCmdId { get; init; }

    public required uint AchievementCmdId { get; init; }

    public required uint DoCmd { get; init; }

    public required uint ToUInt16 { get; init; }

    public required uint UpdateNormalProperty { get; init; }

    public required uint NewString { get; init; }

    public required uint FindGameObject { get; init; }

    public required uint EventSystemUpdate { get; init; }

    public required uint SimulatePointerClick { get; init; }

    public static TargetNativeConfiguration Create(NativeConfiguration config, bool isOversea)
    {
        MethodRva methodRva = isOversea ? config.MethodRva.Oversea : config.MethodRva.Chinese;

        return new()
        {
            StoreCmdId = config.StoreCmdId,
            AchievementCmdId = config.AchievementCmdId,

            // Method RVAs
            DoCmd = methodRva.DoCmd,
            ToUInt16 = methodRva.ToUInt16,
            UpdateNormalProperty = methodRva.UpdateNormalProperty,
            NewString = methodRva.NewString,
            FindGameObject = methodRva.FindGameObject,
            EventSystemUpdate = methodRva.EventSystemUpdate,
            SimulatePointerClick = methodRva.SimulatePointerClick,
        };
    }
}