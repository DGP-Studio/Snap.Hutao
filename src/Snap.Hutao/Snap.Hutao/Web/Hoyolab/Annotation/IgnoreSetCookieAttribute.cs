// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Annotation;

/// <summary>
/// 指示相关的类忽略Http请求的Set-Cookie头
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal class IgnoreSetCookieAttribute : Attribute
{
}