// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Core.Setting;

/// <summary>
/// 本地化
/// </summary>
internal static class Localization
{
    /// <summary>
    /// 初始化本地化语言
    /// </summary>
    /// <param name="culture">语言代码</param>
    public static void Initialize(string culture)
    {
        CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(culture);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }
}