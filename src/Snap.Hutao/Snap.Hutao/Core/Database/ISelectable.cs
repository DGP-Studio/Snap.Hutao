// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 可选择的项
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// 获取或设置当前项的选中状态
    /// </summary>
    bool IsSelected { get; set; }
}