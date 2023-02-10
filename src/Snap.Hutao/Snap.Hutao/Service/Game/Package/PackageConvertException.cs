// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 包转换异常
/// </summary>
public class PackageConvertException : Exception
{
    /// <inheritdoc cref="Exception.Exception(string?, Exception?)"/>
    public PackageConvertException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}