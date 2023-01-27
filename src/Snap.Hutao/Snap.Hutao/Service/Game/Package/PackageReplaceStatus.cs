// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 包更新状态
/// </summary>
public class PackageReplaceStatus
{
    /// <summary>
    /// 构造一个新的包更新状态
    /// </summary>
    /// <param name="description">描述</param>
    public PackageReplaceStatus(string description)
    {
        Description = description;
    }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
}