// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO.Ini;

/// <summary>
/// Ini 序列化器
/// </summary>
internal static class IniSerializer
{
    /// <summary>
    /// 异步反序列化
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <returns>Ini 元素集合</returns>
    public static IEnumerable<IniElement> Deserialize(FileStream fileStream)
    {
        using (TextReader reader = new StreamReader(fileStream))
        {
            while (reader.ReadLine() is string line)
            {
                if (line.Length > 0)
                {
                    if (line[0] == '[')
                    {
                        yield return new IniSection(line[1..^1]);
                    }

                    if (line[0] == ';')
                    {
                        yield return new IniComment(line[1..]);
                    }

                    if (line.IndexOf('=') > 0)
                    {
                        string[] parameters = line.Split('=', 2);
                        yield return new IniParameter(parameters[0], parameters[1]);
                    }
                }

                continue;
            }
        }
    }
}