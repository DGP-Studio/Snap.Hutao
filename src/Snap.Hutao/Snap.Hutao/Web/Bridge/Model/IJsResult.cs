// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// 指示此为Js结果
/// </summary>
[HighQuality]
internal interface IJsResult
{
    /// <summary>
    /// 转换到Json字符串表示
    /// </summary>
    /// <returns>JSON字符串</returns>
    string ToJson();
}
