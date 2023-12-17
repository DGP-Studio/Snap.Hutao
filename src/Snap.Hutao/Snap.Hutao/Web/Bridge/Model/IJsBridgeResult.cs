// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// 指示此为Js结果
/// </summary>
[HighQuality]
internal interface IJsBridgeResult
{
    string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}