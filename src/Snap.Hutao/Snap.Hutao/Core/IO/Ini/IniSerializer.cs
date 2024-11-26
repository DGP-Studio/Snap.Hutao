// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Core.IO.Ini;

internal static class IniSerializer
{
    public static ImmutableArray<IniElement> DeserializeFromFile(string filePath)
    {
        using (StreamReader reader = File.OpenText(filePath))
        {
            return DeserializeCore(reader);
        }
    }

    public static ImmutableArray<IniElement> Deserialize(Stream stream)
    {
        using (StreamReader reader = new(stream))
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

    private static ImmutableArray<IniElement> DeserializeCore(StreamReader reader)
    {
        ImmutableArray<IniElement>.Builder builder = ImmutableArray.CreateBuilder<IniElement>();
        IniSection? currentSection = default;

        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            ReadOnlySpan<char> lineSpan = line;

            if (lineSpan[0] is '[')
            {
                IniSection section = new(lineSpan[1..^1].ToString());
                builder.Add(section);
                currentSection = section;
            }

            if (lineSpan[0] is ';')
            {
                IniComment comment = new(lineSpan[1..].ToString());
                builder.Add(comment);
                currentSection?.Children.Add(comment);
            }

            if (lineSpan.TrySplitIntoTwo('=', out ReadOnlySpan<char> left, out ReadOnlySpan<char> right))
            {
                IniParameter parameter = new(left.Trim().ToString(), right.Trim().ToString());
                builder.Add(parameter);
                currentSection?.Children.Add(parameter);
            }
        }

        return builder.ToImmutable();
    }

    private static void SerializeCore(StreamWriter writer, IEnumerable<IniElement> elements)
    {
        foreach (IniElement element in elements)
        {
            writer.WriteLine(element.ToString());
        }
    }
}