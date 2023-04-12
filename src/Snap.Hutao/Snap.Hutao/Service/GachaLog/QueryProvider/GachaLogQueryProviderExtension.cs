// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 祈愿记录Url提供器拓展
/// </summary>
internal static class GachaLogQueryProviderExtension
{
    /// <summary>
    /// 选出对应的祈愿 Url 提供器
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="option">刷新选项</param>
    /// <returns>对应的祈愿 Url 提供器</returns>
    public static IGachaLogQueryProvider? PickProvider(this IServiceProvider serviceProvider, RefreshOption option)
    {
        IEnumerable<IGachaLogQueryProvider> providers = serviceProvider.GetServices<IGachaLogQueryProvider>();

        string? name = option switch
        {
            RefreshOption.WebCache => nameof(GachaLogQueryWebCacheProvider),
            RefreshOption.SToken => nameof(GachaLogQuerySTokenProvider),
            RefreshOption.ManualInput => nameof(GachaLogQueryManualInputProvider),
            _ => null,
        };

        return providers.SingleOrDefault(p => p.Name == name);
    }
}