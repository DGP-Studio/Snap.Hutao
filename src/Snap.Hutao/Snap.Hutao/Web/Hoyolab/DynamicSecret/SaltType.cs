// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// Salt's type
/// </summary>
public enum SaltType
{
    /// <summary>
    /// 不需要 SALT
    /// </summary>
    None,

    /// <summary>
    /// X4
    /// </summary>
    X4,

    /// <summary>
    /// X6
    /// </summary>
    X6,

    /// <summary>
    /// K2
    /// </summary>
    K2,

    /// <summary>
    /// LK2
    /// </summary>
    LK2,
}