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

    public static TargetNativeConfiguration Create(NativeConfiguration config, bool isOversea)
    {
        MethodRva methodRva = isOversea ? config.MethodRva.Oversea : config.MethodRva.Chinese;

        return new()
        {
            StoreCmdId = config.StoreCmdId,
            AchievementCmdId = config.AchievementCmdId,
            DoCmd = methodRva.DoCmd,
            ToUInt16 = methodRva.ToUInt16,
            UpdateNormalProperty = methodRva.UpdateNormalProperty,
        };
    }
}