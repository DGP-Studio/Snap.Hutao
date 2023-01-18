// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Core;

/// <summary>
/// 支持Md5转换
/// </summary>
internal static class Convert
{
    /// <summary>
    /// 获取字符串的MD5计算结果
    /// </summary>
    /// <param name="source">源字符串</param>
    /// <returns>计算的结果</returns>
    public static string ToMd5HexString(string source)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(source));
        return System.Convert.ToHexString(hash);
    }
}