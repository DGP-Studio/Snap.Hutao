﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 包文件操作的类型
/// </summary>
[HighQuality]
internal enum PackageItemOperationKind
{
    /// <summary>
    /// 需要备份
    /// </summary>
    Backup = 0,

    /// <summary>
    /// 替换
    /// </summary>
    Replace = 1,

    /// <summary>
    /// 添加
    /// </summary>
    Add = 2,
}