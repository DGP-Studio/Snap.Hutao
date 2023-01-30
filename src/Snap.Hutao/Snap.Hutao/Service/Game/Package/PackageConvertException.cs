// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Model.Binding.LaunchGame;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using static Snap.Hutao.Service.Game.GameConstants;

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