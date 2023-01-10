// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Caching;

/// <summary>
/// 图像缓存 文件路径操作
/// </summary>
internal interface IImageCacheFilePathOperation
{
    /// <summary>
    /// 从分类与文件名获取文件路径
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="fileName">文件名</param>
    /// <returns>文件路径</returns>
    string GetFilePathFromCategoryAndFileName(string category, string fileName);
}