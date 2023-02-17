// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 动态密钥创建选项
/// </summary>
[HighQuality]
internal sealed class DynamicSecretCreationOptions
{
    private const string RandomRange = "abcdefghijklmnopqrstuvwxyz1234567890";

    private readonly bool includeChars;

    /// <summary>
    /// 构造一个新的动态密钥创建选项
    /// </summary>
    /// <param name="option">选项字符串</param>
    public DynamicSecretCreationOptions(string option)
    {
        string[] definations = option.Split('|');
        string version = definations[0];
        string saltType = definations[1];
        string includeChars = definations[2];

        Version = Enum.Parse<DynamicSecretVersion>(version);
        SaltType = Enum.Parse<SaltType>(saltType);
        this.includeChars = bool.Parse(includeChars);
    }

    /// <summary>
    /// DS 版本
    /// </summary>
    public DynamicSecretVersion Version { get; }

    /// <summary>
    /// SALT 类型
    /// </summary>
    public SaltType SaltType { get; }

    /// <summary>
    /// 默认 Body
    /// </summary>
    public string DefaultBody
    {
        get => SaltType == SaltType.PROD ? "{}" : string.Empty;
    }

    /// <summary>
    /// 随机字符串
    /// </summary>
    public string RandomString
    {
        get => includeChars ? GetRandomStringWithChars() : GetRandomStringNoChars();
    }

    private static string GetRandomStringWithChars()
    {
        StringBuilder sb = new(6);

        for (int i = 0; i < 6; i++)
        {
            int pos = Random.Shared.Next(0, RandomRange.Length);
            sb.Append(RandomRange[pos]);
        }

        return sb.ToString();
    }

    private static string GetRandomStringNoChars()
    {
        int rand = Random.Shared.Next(100000, 200000);
        return $"{(rand == 100000 ? 642367 : rand)}";
    }
}