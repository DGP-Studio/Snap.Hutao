// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 包操作
/// </summary>
[HighQuality]
[DebuggerDisplay("Action:{Type} Target:{Target} Cache:{Cache}")]
internal readonly struct PackageItemOperationInfo
{
    /// <summary>
    /// 操作的类型
    /// </summary>
    public readonly PackageItemOperationType Type;

    /// <summary>
    /// 目标文件
    /// </summary>
    public readonly VersionItem Remote;

    /// <summary>
    /// 移动至中时的名称
    /// </summary>
    public readonly VersionItem Local;

    /// <summary>
    /// 构造一个新的包操作
    /// </summary>
    /// <param name="type">操作类型</param>
    /// <param name="remote">远程</param>
    /// <param name="local">本地</param>
    public PackageItemOperationInfo(PackageItemOperationType type, VersionItem remote, VersionItem local)
    {
        Type = type;
        Remote = remote;
        Local = local;
    }
}