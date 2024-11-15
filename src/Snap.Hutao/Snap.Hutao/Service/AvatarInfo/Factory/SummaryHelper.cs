// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

internal static class SummaryHelper
{
    public static ReliquarySubAffixId GetAffixMaxId(in ReliquarySubAffixId appendId)
    {
        // axxxxx -> a
        uint value = appendId / 100000U;

        // a -> m
        uint max = value switch
        {
            1 => 2,
            2 => 3,
            3 or 4 or 5 => 4,
            _ => throw HutaoException.Throw($"不支持的 ReliquarySubAffixId: {appendId}"),
        };

        // axxxxb -> axxxx -> axxxx0 -> axxxxm
        return ((appendId / 10) * 10) + max;
    }

    public static float GetPercentSubAffixScore(in ReliquarySubAffixId appendId)
    {
        // 圣遗物相同类型副词条强化档位一共为 4/3/2 档
        // 五星 为 70% 80% 90% 100%
        // 四星 为 70% 80% 90% 100%
        // 三星 为 70% 80% 90% 100%
        // 二星 为 70%   85%   100%
        // 二星 为 80%         100%
        // 通过计算与最大属性的 Id 差来决定当前副词条的强化档位
        uint maxId = GetAffixMaxId(appendId);
        uint delta = maxId - appendId;

        return (maxId / 100000, delta) switch
        {
            (5 or 4 or 3, 0) => 100F,
            (5 or 4 or 3, 1) => 90F,
            (5 or 4 or 3, 2) => 80F,
            (5 or 4 or 3, 3) => 70F,

            (2, 0) => 100F,
            (2, 1) => 85F,
            (2, 2) => 70F,

            (1, 0) => 100F,
            (1, 1) => 80F,

            _ => throw HutaoException.Throw($"Unexpected AppendId: {appendId} Delta: {delta}"),
        };
    }
}