// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.GachaLog;

/// <summary>
/// UIGF版本
/// </summary>
internal enum UIGFVersion
{
    /// <summary>
    /// 不支持的版本
    /// </summary>
    NotSupported,

    /// <summary>
    /// v2.2以及之前的版本
    /// </summary>
    Major2Minor2OrLower,

    /// <summary>
    /// v2.3以及之后的版本
    /// </summary>
    Major2Minor3OrHigher,
}