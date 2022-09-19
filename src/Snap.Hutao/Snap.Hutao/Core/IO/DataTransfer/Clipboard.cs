// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;
using Windows.ApplicationModel.DataTransfer;

namespace Snap.Hutao.Core.IO.DataTransfer;

/// <summary>
/// 剪贴板
/// </summary>
internal static class Clipboard
{
    /// <summary>
    /// 从剪贴板文本中反序列化
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="options">Json序列化选项</param>
    /// <returns>实例</returns>
    public static async Task<T?> DeserializeTextAsync<T>(JsonSerializerOptions options)
        where T : class
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        DataPackageView view = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
        string json = await view.GetTextAsync();
        return JsonSerializer.Deserialize<T>(json, options);
    }
}
