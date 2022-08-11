// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Convert;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 为MiHoYo接口请求器 <see cref="Requester"/> 提供动态密钥
/// </summary>
internal abstract class DynamicSecretProvider : Md5Convert
{
    /// <summary>
    /// 创建动态密钥
    /// </summary>
    /// <returns>密钥</returns>
    public static string Create()
    {
        // unix timestamp
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        string r = GetRandomString();

        string check = ToHexString($"salt={Core.CoreEnvironment.DynamicSecret1Salt}&t={t}&r={r}").ToLowerInvariant();

        return $"{t},{r},{check}";
    }

    private static string GetRandomString()
    {
        StringBuilder sb = new(6);

        for (int i = 0; i < 6; i++)
        {
            int v8 = Random.Shared.Next(0, 32768) % 26;
            int v9 = 87;
            if (v8 < 10)
            {
                v9 = 48;
            }

            _ = sb.Append((char)(v8 + v9));
        }

        return sb.ToString();
    }
}
