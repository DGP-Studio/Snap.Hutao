// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Hutao.Web.Hoyolab.Annotation;

/// <summary>
/// API 信息
/// </summary>
/// <typeparam name="TReturnType">API 的返回类型</typeparam>
[AttributeUsage(AttributeTargets.Method)]
internal class ApiInformationAttribute<TReturnType> : Attribute
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