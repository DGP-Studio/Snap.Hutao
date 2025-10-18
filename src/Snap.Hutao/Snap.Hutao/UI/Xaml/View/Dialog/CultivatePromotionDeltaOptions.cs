// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

internal sealed class CultivatePromotionDeltaOptions
{
    public CultivatePromotionDeltaOptions(AvatarPromotionDelta delta, ConsumptionSaveStrategyKind strategy)
    {
        delta.AvatarLevelTarget = delta.AvatarLevelTarget switch
        {
            >= 100 => 100,
            >= 95 => 95,
            _ => delta.AvatarLevelTarget
        };

        Delta = delta;
        Strategy = strategy;
    }

    public AvatarPromotionDelta Delta { get; }

    public ConsumptionSaveStrategyKind Strategy { get; }
}