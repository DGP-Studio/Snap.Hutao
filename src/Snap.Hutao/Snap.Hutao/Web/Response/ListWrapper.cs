// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

/// <summary>
/// 列表对象包装器
/// </summary>
/// <typeparam name="T">列表的元素类型</typeparam>
[HighQuality]
internal class ListWrapper<T>
{
    /// <summary>
    /// 列表
    /// </summary>
    [JsonPropertyName("list")]
    public List<T> List { get; set; } = default!;
}