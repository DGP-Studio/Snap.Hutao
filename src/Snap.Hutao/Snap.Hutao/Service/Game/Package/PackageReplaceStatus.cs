// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 包更新状态
/// </summary>
internal sealed class PackageReplaceStatus
{
    /// <summary>
    /// 构造一个新的包更新状态
    /// </summary>
    /// <param name="name">描述</param>
    public PackageReplaceStatus(string name)
    {
        Name = name;
        Description = default!;
    }

    /// <summary>
    /// 构造一个新的包更新状态
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="bytesRead">读取的字节数</param>
    /// <param name="totalBytes">总字节数</param>
    public PackageReplaceStatus(string name, long bytesRead, long totalBytes)
    {
        Percent = (double)bytesRead / totalBytes;
        Name = name;
        Description = $"{Converters.ToFileSizeString(bytesRead)}/{Converters.ToFileSizeString(totalBytes)}";
    }

    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 进度
    /// </summary>
    public double Percent { get; set; } = -1;

    /// <summary>
    /// 是否有进度
    /// </summary>
    public bool IsIndeterminate { get => Percent < 0; }
}