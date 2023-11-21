﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage.Streams;

namespace Snap.Hutao.Core.IO.DataTransfer;

/// <summary>
/// 剪贴板互操作
/// </summary>
internal interface IClipboardProvider
{
    /// <summary>
    /// 从剪贴板文本中反序列化
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <returns>实例</returns>
    ValueTask<T?> DeserializeFromJsonAsync<T>()
        where T : class;

    /// <summary>
    /// 设置位图
    /// </summary>
    /// <param name="stream">图片流</param>
    /// <returns>是否设置成功</returns>
    bool SetBitmap(IRandomAccessStream stream);

    /// <summary>
    /// 设置文本
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>是否设置成功</returns>
    bool SetText(string text);
}