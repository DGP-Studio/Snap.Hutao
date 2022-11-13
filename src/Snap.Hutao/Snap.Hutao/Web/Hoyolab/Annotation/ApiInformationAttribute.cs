// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.DynamicSecret;

namespace Snap.Hutao.Web.Hoyolab.Annotation;

/// <summary>
/// API 信息
/// 指示此API 已经经过验证，且明确其调用
/// </summary>
/// <typeparam name="TReturnType">API 的返回类型</typeparam>
[AttributeUsage(AttributeTargets.Method)]
internal class ApiInformationAttribute : Attribute
{
    /// <summary>
    /// Cookie类型
    /// </summary>
    public CookieType Cookie { get; set; }

    /// <summary>
    /// SALT
    /// </summary>
    public SaltType Salt { get; set; }
}