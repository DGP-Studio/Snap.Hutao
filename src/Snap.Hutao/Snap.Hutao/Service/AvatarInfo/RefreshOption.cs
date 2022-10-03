// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo;

/// <summary>
/// 刷新选项
/// </summary>
[Flags]
public enum RefreshOption
{
    /// <summary>
    /// 是否存入本地数据库
    /// </summary>
    StoreInDatabase = 0b00000001,

    /// <summary>
    /// 从API获取
    /// </summary>
    RequestFromAPI = 0b00000010,

    /// <summary>
    /// 仅数据库
    /// </summary>
    DatabaseOnly = 0b00000000,

    /// <summary>
    /// 标准操作
    /// </summary>
    Standard = StoreInDatabase | RequestFromAPI,
}