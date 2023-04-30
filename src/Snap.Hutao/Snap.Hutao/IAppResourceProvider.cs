// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao;

/// <summary>
/// 应用程序资源提供器
/// </summary>
internal interface IAppResourceProvider
{
    /// <summary>
    /// 获取资源
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="name">资源的名称</param>
    /// <returns>资源</returns>
    T GetResource<T>(string name);
}