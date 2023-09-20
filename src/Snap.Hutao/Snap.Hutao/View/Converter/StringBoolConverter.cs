// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Converters;
using Snap.Hutao.Control;

namespace Snap.Hutao.View.Converter;

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
        EmptyValue = BoxedValues.False;
        NotEmptyValue = BoxedValues.True;
    }
}
