// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// <see cref="AppActivationArguments"/> 扩展
/// </summary>
public static class AppActivationArgumentsExtensions
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
        if (activatedEventArgs.Data is IProtocolActivatedEventArgs protocolArgs)
        {
            uri = protocolArgs.Uri;
            return true;
        }

        return false;
    }
}
