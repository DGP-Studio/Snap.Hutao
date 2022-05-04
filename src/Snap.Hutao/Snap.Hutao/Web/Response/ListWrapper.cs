// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Snap.Hutao.Web.Response;

/// <summary>
/// 列表对象包装器
/// </summary>
/// <typeparam name="T">列表的元素类型</typeparam>
public class ListWrapper<T>
{
    /// <summary>
    /// 列表
    /// </summary>
    [JsonProperty("list")] public List<T>? List { get; set; }
}
