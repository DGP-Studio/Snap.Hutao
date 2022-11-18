// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 动态密钥版本
/// </summary>
public enum DynamicSecretVersion
{
    /// <summary>
    /// 一代
    /// </summary>
    Gen1,

    /// <summary>
    /// 二代，额外添加了b，q 参数
    /// </summary>
    Gen2,
}
