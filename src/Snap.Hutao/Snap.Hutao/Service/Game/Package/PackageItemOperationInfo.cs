// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Package;

[DebuggerDisplay("Action:{Kind} Remote:{Remote} Local:{Local}")]
internal readonly struct PackageItemOperationInfo
{
    public readonly PackageItemOperationKind Kind;
    public readonly VersionItem Remote;
    public readonly VersionItem Local;

    public PackageItemOperationInfo(PackageItemOperationKind kind, VersionItem remote, VersionItem local)
    {
        Kind = kind;
        Remote = remote;
        Local = local;
    }
}