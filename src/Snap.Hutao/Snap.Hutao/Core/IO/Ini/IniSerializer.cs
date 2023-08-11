// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO.Ini;

/// <summary>
/// Ini 序列化器
/// </summary>
[HighQuality]
internal static class IniSerializer
{
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <returns>Ini 元素集合</returns>
    public static IEnumerable<IniElement> Deserialize(FileStream fileStream)
    {
        using (StreamReader reader = new(fileStream))
        {
            while (reader.ReadLine() is { } line)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                ReadOnlySpan<char> lineSpan = line;

                if (lineSpan[0] is '[')
                {
                    yield return new IniSection(lineSpan[1..^1].ToString());
                }

                if (lineSpan[0] is ';')
                {
                    yield return new IniComment(lineSpan[1..].ToString());
                }

                if (lineSpan.TrySplitIntoTwo('=', out ReadOnlySpan<char> left, out ReadOnlySpan<char> right))
                {
                    yield return new IniParameter(left.Trim().ToString(), right.Trim().ToString());
                }
            }
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="fileStream">写入的流</param>
    /// <param name="elements">元素</param>
    public static void Serialize(FileStream fileStream, IEnumerable<IniElement> elements)
    {
        using (StreamWriter writer = new(fileStream))
        {
            foreach (IniElement element in elements)
            {
                writer.WriteLine(element.ToString());
            }
        }
    }
}