// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;
using Snap.Hutao.Extension;
using Snap.Hutao.Localization;
using System.Globalization;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// 国际化拓展
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
internal class I18NExtension : MarkupExtension
{
    private static readonly ITranslation Translation;

    private static readonly Dictionary<string, Type> TranslationMap = new()
    {
        ["zh-CN"] = typeof(LanguagezhCN),
    };

    static I18NExtension()
    {
        string currentName = CultureInfo.CurrentUICulture.Name;
        Type? languageType = ((IDictionary<string, Type>)TranslationMap).GetValueOrDefault2(currentName, typeof(LanguagezhCN));
        Translation = (ITranslation)Activator.CreateInstance(languageType!)!;
    }

    /// <summary>
    /// 构造默认的国际化拓展
    /// </summary>
    public I18NExtension()
        : base()
    {
    }

    /// <summary>
    /// 构造默认的国际化拓展
    /// </summary>
    /// <param name="key">键</param>
    public I18NExtension(string key)
    {
        Key = key;
    }

    /// <summary>
    /// 键名称
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    /// 获取字符串
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>翻译的字符串</returns>
    internal static string Get(string key)
    {
        return Translation[key];
    }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return Translation[Key];
    }
}