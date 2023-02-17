// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 动态密钥创建选项拓展
/// </summary>
[HighQuality]
internal static class HttpRequestMessageDynamicSecretExtension
{
    private static readonly ConditionalWeakTable<HttpRequestMessage, DynamicSecretCreationOptions> OptionMap = new();

    /// <summary>
    /// 尝试获取创建选项
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="options">选项</param>
    /// <returns>该请求是否包含创建选项</returns>
    public static bool TryGetDynamicSecretCreationOptions(this HttpRequestMessage request, [NotNullWhen(true)] out DynamicSecretCreationOptions? options)
    {
        return OptionMap.TryGetValue(request, out options);
    }

    /// <summary>
    /// 设置创建选项
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="options">选项</param>
    public static void SetDynamicSecretCreationOptions(this HttpRequestMessage request, DynamicSecretCreationOptions options)
    {
        OptionMap.AddOrUpdate(request, options);
    }
}