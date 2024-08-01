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
    public static List<IniElement> DeserializeFromFile(string filePath)
    {
        using (StreamReader reader = File.OpenText(filePath))
        {
            return DeserializeCore(reader);
        }
    }

    public static List<IniElement> Deserialize(Stream fileStream)
    {
        using (StreamReader reader = new(fileStream))
        {
            return DeserializeCore(reader);
        }
    }

    public static void SerializeToFile(string filePath, IEnumerable<IniElement> elements)
    {
        using (StreamWriter writer = File.CreateText(filePath))
        {
            SerializeCore(writer, elements);
        }
    }

    public static void Serialize(FileStream fileStream, IEnumerable<IniElement> elements)
    {
        using (StreamWriter writer = new(fileStream))
        {
            SerializeCore(writer, elements);
        }
    }

    private static List<IniElement> DeserializeCore(StreamReader reader)
    {
        List<IniElement> results = [];
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            ReadOnlySpan<char> lineSpan = line;

            if (lineSpan[0] is '[')
            {
                results.Add(new IniSection(lineSpan[1..^1].ToString()));
            }

            if (lineSpan[0] is ';')
            {
                results.Add(new IniComment(lineSpan[1..].ToString()));
            }

            if (lineSpan.TrySplitIntoTwo('=', out ReadOnlySpan<char> left, out ReadOnlySpan<char> right))
            {
                results.Add(new IniParameter(left.Trim().ToString(), right.Trim().ToString()));
            }
        }

        return results;
    }

    private static void SerializeCore(StreamWriter writer, IEnumerable<IniElement> elements)
    {
        foreach (IniElement element in elements)
        {
            writer.WriteLine(element.ToString());
        }
    }
}