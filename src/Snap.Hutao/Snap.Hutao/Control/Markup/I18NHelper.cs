// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// 国际化帮助类
/// WinUI 3 目前存在部分页面无法使用 MarkupExtension 的问题
/// 使用 此帮助类 绕过限制
/// </summary>
internal class I18NHelper
{
    private static readonly DependencyProperty TranslationProperty = Property<I18NHelper>.Attach("Translation", string.Empty, OnKeyChanged);

    /// <summary>
    /// 获取键
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns>值</returns>
    public static string GetTranslation(DependencyObject obj)
    {
        return (string)obj.GetValue(TranslationProperty);
    }

    /// <summary>
    /// 设置键
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="value">值</param>
    public static void SetTranslation(DependencyObject obj, string value)
    {
        string tarnslation = I18NExtension.Get(value);
        obj.SetValue(TranslationProperty, tarnslation);
    }

    private static void OnKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs arg)
    {
        string translation = I18NExtension.Get(arg.NewValue.ToString() ?? string.Empty);

        if (obj is AppBarButton appBarButton)
        {
            appBarButton.Label = translation;
        }
        else if (obj is AppBarToggleButton appBarToggleButton)
        {
            appBarToggleButton.Label = translation;
        }
        else if (obj is AutoSuggestBox autoSuggestBox)
        {
            autoSuggestBox.PlaceholderText = translation;
        }
        else if (obj is ContentControl contentControl)
        {
            contentControl.Content = translation;
        }
        else if (obj is MenuFlyoutItem menuFlyoutItem)
        {
            menuFlyoutItem.Text = translation;
        }
        else if (obj is TextBlock textBlock)
        {
            textBlock.Text = translation;
        }
    }
}