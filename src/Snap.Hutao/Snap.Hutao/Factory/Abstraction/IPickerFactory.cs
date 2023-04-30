// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage.Pickers;

namespace Snap.Hutao.Factory.Abstraction;

/// <summary>
/// 文件选择器工厂
/// </summary>
[HighQuality]
internal interface IPickerFactory
{
    /// <summary>
    /// 获取 经过初始化的 <see cref="FileOpenPicker"/>
    /// </summary>
    /// <param name="location">初始位置</param>
    /// <param name="commitButton">提交按钮文本</param>
    /// <param name="fileTypes">文件类型</param>
    /// <returns>经过初始化的 <see cref="FileOpenPicker"/></returns>
    FileOpenPicker GetFileOpenPicker(PickerLocationId location, string commitButton, params string[] fileTypes);

    /// <summary>
    /// 获取 经过初始化的 <see cref="FileSavePicker"/>
    /// </summary>
    /// <returns>经过初始化的 <see cref="FileSavePicker"/></returns>
    [Obsolete]
    FileSavePicker GetFileSavePicker();

    /// <summary>
    /// 获取 经过初始化的 <see cref="FileSavePicker"/>
    /// </summary>
    /// <param name="location">初始位置</param>
    /// <param name="fileName">文件名</param>
    /// <param name="commitButton">提交按钮文本</param>
    /// <param name="fileTypes">文件类型</param>
    /// <returns>经过初始化的 <see cref="FileSavePicker"/></returns>
    FileSavePicker GetFileSavePicker(PickerLocationId location, string fileName, string commitButton, IDictionary<string, IList<string>> fileTypes);

    /// <summary>
    /// 获取 经过初始化的 <see cref="FolderPicker"/>
    /// </summary>
    /// <returns>经过初始化的 <see cref="FolderPicker"/></returns>
    FolderPicker GetFolderPicker();
}