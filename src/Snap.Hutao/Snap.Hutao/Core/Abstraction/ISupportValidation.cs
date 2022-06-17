// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Abstraction;

/// <summary>
/// 表示支持验证
/// </summary>
internal interface ISupportValidation
{
    /// <summary>
    /// 验证
    /// </summary>
    /// <returns>当前数据是否有效</returns>
    public bool Validate();
}
