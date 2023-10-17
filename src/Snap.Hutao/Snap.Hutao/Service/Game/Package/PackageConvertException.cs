// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 包转换异常
/// </summary>
[HighQuality]
internal sealed class PackageConvertException : Exception
{
    /// <inheritdoc cref="Exception(string?, Exception?)"/>
    public PackageConvertException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}