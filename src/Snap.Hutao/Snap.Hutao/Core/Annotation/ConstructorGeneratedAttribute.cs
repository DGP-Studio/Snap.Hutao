// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Annotation;

/// <summary>
/// 指示此类自动生成构造器
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal sealed class ConstructorGeneratedAttribute : Attribute
{
    /// <summary>
    /// 指示此类自动生成构造器
    /// </summary>
    public ConstructorGeneratedAttribute()
    {
    }

    /// <summary>
    /// 在构造函数中插入 HttpClient
    /// </summary>
    public bool ResolveHttpClient { get; set; }
}