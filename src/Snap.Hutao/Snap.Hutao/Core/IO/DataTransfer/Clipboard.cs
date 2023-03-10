// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace Snap.Hutao.Core.IO.DataTransfer;

/// <summary>
/// 剪贴板 在主线程使用
/// </summary>
[HighQuality]
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

        if (view.Contains(StandardDataFormats.Text))
        {
            string json = await view.GetTextAsync();

            await ThreadHelper.SwitchToBackgroundAsync();
            return JsonSerializer.Deserialize<T>(json, options);
        }

        return null;
    }

    /// <summary>
    /// 设置文本
    /// </summary>
    /// <param name="text">文本</param>
    public static void SetText(string text)
    {
        DataPackage content = new() { RequestedOperation = DataPackageOperation.Copy };
        content.SetText(text);
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
        Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
    }

    /// <summary>
    /// 设置位图
    /// </summary>
    /// <param name="stream">位图流</param>
    public static void SetBitmap(IRandomAccessStream stream)
    {
        RandomAccessStreamReference reference = RandomAccessStreamReference.CreateFromStream(stream);
        DataPackage content = new() { RequestedOperation = DataPackageOperation.Copy };
        content.SetBitmap(reference);
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
        Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
    }

    /// <summary>
    /// 设置位图
    /// </summary>
    /// <param name="file">文件</param>
    public static void SetBitmap(string file)
    {
        using (IRandomAccessStream stream = File.OpenRead(file).AsRandomAccessStream())
        {
            SetBitmap(stream);
        }
    }
}
