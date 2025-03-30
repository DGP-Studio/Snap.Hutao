// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Cultivation;

internal sealed class MaterialIdComparer : IComparer<MaterialId>
{
    private static MaterialIdComparer? shared;

    public static MaterialIdComparer Shared { get => LazyInitializer.EnsureInitialized(ref shared); }

    public int Compare(MaterialId x, MaterialId y)
    {
        return Transform(x).CompareTo(Transform(y));
    }

    private static uint Transform(MaterialId value)
    {
        return value.Value switch
        {
            // 摩拉
            202U => 0U,

            // 经验
            104001U => 1U,
            104002U => 2U,
            104003U => 3U,

            // 魔矿
            104011U => 4U,
            104012U => 5U,
            104013U => 6U,

            _ => value,
        };
    }
}