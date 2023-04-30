// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 元素精通系数
/// </summary>
[HighQuality]
internal readonly struct ElementMasteryCoefficient
{
    /// <summary>
    /// 参数1
    /// </summary>
    public readonly float Param1;

    /// <summary>
    /// 参数2
    /// </summary>
    public readonly float Param2;

    /// <summary>
    /// 构造一个新的元素精通系数
    /// </summary>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    public ElementMasteryCoefficient(float p1, float p2)
    {
        Param1 = p1;
        Param2 = p2;
    }
}
