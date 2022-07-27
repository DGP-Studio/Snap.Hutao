// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 包装一个描述参数
/// 专用于绑定
/// </summary>
public class ParameterInfo
{
    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 参数
    /// </summary>
    public string Parameter { get; set; } = default!;
}