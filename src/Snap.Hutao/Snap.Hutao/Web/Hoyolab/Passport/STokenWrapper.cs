// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// SToken 包装器
/// </summary>
internal sealed class STokenWrapper
{
    /// <summary>
    /// 构造一个新的SToken 包装器
    /// </summary>
    /// <param name="stoken">stoken</param>
    /// <param name="uid">uid</param>
    public STokenWrapper(string stoken, string uid)
    {
        SToken = stoken;
        Uid = uid;
    }

    /// <summary>
    /// SToken
    /// </summary>
    [JsonPropertyName("stoken")]
    public string SToken { get; set; }

    /// <summary>
    /// Uid
    /// </summary>
    [JsonPropertyName("uid")]
    public string Uid { get; set; }
}