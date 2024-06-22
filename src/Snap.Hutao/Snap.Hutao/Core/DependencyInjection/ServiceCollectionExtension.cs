// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 服务管理器
/// 依赖注入的核心管理类
/// </summary>
[HighQuality]
internal static partial class ServiceCollectionExtension
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial IServiceCollection AddInjections(this IServiceCollection services);
}