// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 体型
/// </summary>
public enum BodyType
{
    /// <summary>
    /// 无
    /// </summary>
    BODY_NONE,

    /// <summary>
    /// 男孩
    /// </summary>
    [Description("少男")]
    BODY_BOY,

    /// <summary>
    /// 女孩
    /// </summary>
    [Description("少女")]
    BODY_GIRL,

    /// <summary>
    /// 成女
    /// </summary>
    [Description("成女")]
    BODY_LADY,

    /// <summary>
    /// 成男
    /// </summary>
    [Description("成男")]
    BODY_MALE,

    /// <summary>
    /// 萝莉
    /// </summary>
    [Description("萝莉")]
    BODY_LOLI,
}