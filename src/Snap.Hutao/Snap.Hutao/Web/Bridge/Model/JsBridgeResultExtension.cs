// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

internal static class JsBridgeResultExtension
{
    public static string ToJson<T>(this T result)
        where T : IJsBridgeResult
    {
        return JsonSerializer.Serialize(result, result.GetType());
    }
}