// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 名称与描述
/// </summary>
internal class NameDescription
{
    /// <summary>
    /// 构造一个空的名称与描述
    /// </summary>
    public NameDescription()
    {
    }

    /// <summary>
    /// 构造一个新的名称与描述
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="description">描述</param>
    public NameDescription(string name, string description)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;
}