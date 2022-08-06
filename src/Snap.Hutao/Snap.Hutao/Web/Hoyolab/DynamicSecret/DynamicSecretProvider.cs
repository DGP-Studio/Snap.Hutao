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
    // @Azure99 respect original author
    private static readonly string Salt = "4a8knnbk5pbjqsrudp3dq484m9axoc5g";

    /// <summary>
    /// 创建动态密钥
    /// </summary>
    /// <returns>密钥</returns>
    public static string Create()
    {
        // unix timestamp
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string r = GetRandomString();
        string check = ToHexString($"salt={Salt}&t={t}&r={r}");

        return $"{t},{r},{check}";
    }

    private static string GetRandomString()
    {
        StringBuilder sb = new(6);
        Random random = new();

        for (int i = 0; i < 6; i++)
        {
            int offset = random.Next(0, 32768) % 26;

            // 实际上只能取到前16个小写字母
            int target = 'a' - 10;

            // 取数字
            if (offset < 10)
            {
                target = '0';
            }

            _ = sb.Append((char)(offset + target));
        }

        return sb.ToString();
    }
}
