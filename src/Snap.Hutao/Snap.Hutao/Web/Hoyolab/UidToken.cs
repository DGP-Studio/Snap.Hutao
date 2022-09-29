// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// Uid Token 对
/// </summary>
public struct UidToken
{
    /// <summary>
    /// Uid
    /// </summary>
    public string Uid;

    /// <summary>
    /// Token
    /// </summary>
    public string Token;

    /// <summary>
    /// 构造一个新的 Uid Token 对
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">token</param>
    public UidToken(string uid, string token)
    {
        Uid = uid;
        Token = token;
    }
}