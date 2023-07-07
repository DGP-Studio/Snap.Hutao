// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// <see cref="AppActivationArguments"/> 扩展
/// </summary>
[HighQuality]
internal static class AppActivationArgumentsExtensions
{
    /// <summary>
    /// 尝试获取协议启动的Uri
    /// </summary>
    /// <param name="activatedEventArgs">应用程序激活参数</param>
    /// <param name="uri">协议Uri</param>
    /// <returns>是否存在协议Uri</returns>
    public static bool TryGetProtocolActivatedUri(this AppActivationArguments activatedEventArgs, [NotNullWhen(true)] out Uri? uri)
    {
        uri = null;
        if (activatedEventArgs.Data is not IProtocolActivatedEventArgs protocolArgs)
        {
            return false;
        }

        uri = protocolArgs.Uri;
        return true;
    }

    /// <summary>
    /// 尝试获取启动的参数
    /// </summary>
    /// <param name="activatedEventArgs">应用程序激活参数</param>
    /// <param name="arguments">参数</param>
    /// <returns>是否存在参数</returns>
    public static bool TryGetLaunchActivatedArgument(this AppActivationArguments activatedEventArgs, [NotNullWhen(true)] out string? arguments)
    {
        arguments = null;
        if (activatedEventArgs.Data is not ILaunchActivatedEventArgs launchArgs)
        {
            return false;
        }

        arguments = launchArgs.Arguments.Trim();
        return true;
    }
}