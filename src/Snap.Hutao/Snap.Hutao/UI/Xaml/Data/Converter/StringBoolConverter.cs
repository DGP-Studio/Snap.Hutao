// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Converters;

namespace Snap.Hutao.UI.Xaml.Data.Converter;

/// <summary>
/// 字符串空检查转换器
/// </summary>
internal sealed class StringBoolConverter : EmptyStringToObjectConverter
{
    /// <summary>
    /// 构造一个新的字符串空检查转换器
    /// </summary>
    public StringBoolConverter()
    {
        EmptyValue = false;
        NotEmptyValue = true;
    }
}
