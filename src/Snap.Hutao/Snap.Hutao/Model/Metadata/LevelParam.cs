// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 等级与参数
/// </summary>
/// <typeparam name="TLevel">等级的类型</typeparam>
/// <typeparam name="TParam">参数的类型</typeparam>
public class LevelParam<TLevel, TParam>
{
    /// <summary>
    /// 等级
    /// </summary>
    public TLevel Level { get; set; } = default!;

    /// <summary>
    /// 参数
    /// </summary>
    public IList<TParam> Parameters { get; set; } = default!;
}