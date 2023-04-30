// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 包操作
/// </summary>
[HighQuality]
[DebuggerDisplay("Action:{Type} Target:{Target} Cache:{Cache}")]
internal readonly struct ItemOperationInfo
{
    /// <summary>
    /// 操作的类型
    /// </summary>
    public readonly ItemOperationType Type;

    /// <summary>
    /// 目标文件
    /// </summary>
    public readonly string Target;

    /// <summary>
    /// 移动至中时的名称
    /// </summary>
    public readonly string MoveTo;

    /// <summary>
    /// 文件的目标Md5
    /// </summary>
    public readonly string Md5;

    /// <summary>
    /// 文件的目标大小 Byte
    /// </summary>
    public readonly long TotalBytes;

    /// <summary>
    /// 构造一个新的包操作
    /// </summary>
    /// <param name="type">操作类型</param>
    /// <param name="target">目标</param>
    /// <param name="cache">缓存</param>
    public ItemOperationInfo(ItemOperationType type, VersionItem target, VersionItem cache)
    {
        Type = type;
        Target = target.RemoteName;
        MoveTo = cache.RemoteName;
        Md5 = target.Md5;
        TotalBytes = target.FileSize;
    }
}