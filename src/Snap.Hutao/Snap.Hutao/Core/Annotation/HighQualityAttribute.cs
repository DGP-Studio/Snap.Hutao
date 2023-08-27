// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.Annotation;

/// <summary>
/// 高质量代码
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false)]
[Conditional("DEBUG")]
internal sealed class HighQualityAttribute : Attribute
{
}