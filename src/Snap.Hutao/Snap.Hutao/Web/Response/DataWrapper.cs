// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

/// <summary>
/// 数据对象包装器
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
internal sealed class DataWrapper<T>
{
    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}