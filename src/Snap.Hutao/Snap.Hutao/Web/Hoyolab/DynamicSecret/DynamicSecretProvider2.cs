// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Convert;
using System.Linq;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 为MiHoYo接口请求器 <see cref="Requester"/> 提供2代动态密钥
/// </summary>
internal abstract class DynamicSecretProvider2 : Md5Convert
{
    /// <summary>
    /// 创建动态密钥
    /// </summary>
    /// <param name="options">json格式化器</param>
    /// <param name="queryUrl">查询url</param>
    /// <param name="postBody">请求体</param>
    /// <returns>密钥</returns>
    public static string Create(JsonSerializerOptions options, string queryUrl, object? postBody = null)
    {
        // unix timestamp
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // random
        string r = GetRandomString();

        // body
        string b = postBody is null ? string.Empty : JsonSerializer.Serialize(postBody, options);

        // query
        string q = string.Join("&", new UriBuilder(queryUrl).Query.Split('&').OrderBy(x => x));

        // check
        string check = ToHexString($"salt={Core.CoreEnvironment.DynamicSecret2Salt}&t={t}&r={r}&b={b}&q={q}").ToLowerInvariant();

        return $"{t},{r},{check}";
    }

    private static string GetRandomString()
    {
        // 原汁原味
        // v16 = time(0LL);
        // srand(v16);
        // v17 = (int)((double)rand() / 2147483650.0 * 100000.0 + 100000.0) % 1000000;
        // if (v17 >= 100001)
        //     v18 = v17;
        // else
        //     v18 = v17 + 542367;
        int rand = Random.Shared.Next(100000, 200000);
        if (rand == 100000)
        {
            rand = 642367;
        }

        return rand.ToString();
    }
}