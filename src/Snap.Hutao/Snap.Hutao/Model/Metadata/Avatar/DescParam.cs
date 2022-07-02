// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 描述与参数
/// </summary>
public class DescParam
{
    /// <summary>
    /// 描述
    /// </summary>
    public IEnumerable<string> Descriptions { get; set; } = default!;

    /// <summary>
    /// 参数
    /// </summary>
    public IEnumerable<LevelParam<int>> Parameters { get; set; } = default!;
}