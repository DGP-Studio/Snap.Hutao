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
            return PrivateDeserialize(reader);
        }
    }

    public static ImmutableArray<IniElement> Deserialize(Stream stream)
    {
        using (StreamReader reader = new(stream))
        {
            return PrivateDeserialize(reader);
        }
    }

    public static void SerializeToFile(string filePath, IEnumerable<IniElement> elements)
    {
        using (StreamWriter writer = File.CreateText(filePath))
        {
            PrivateSerialize(writer, elements);
        }
    }

    public static void Serialize(FileStream fileStream, IEnumerable<IniElement> elements)
    {
        using (StreamWriter writer = new(fileStream))
        {
            PrivateSerialize(writer, elements);
        }
    }

    private static ImmutableArray<IniElement> PrivateDeserialize(StreamReader reader)
    {
        ImmutableArray<IniElement>.Builder builder = ImmutableArray.CreateBuilder<IniElement>();
        IniSection.Builder? currentSectionBuilder = default;

        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            ReadOnlySpan<char> lineSpan = line;

            if (lineSpan[0] is '[')
            {
                if (currentSectionBuilder is not null)
                {
                    IniSection section = currentSectionBuilder.ToSection();
                    builder.Add(section);
                    builder.AddRange(section.Children);
                }

                currentSectionBuilder = new(lineSpan[1..^1].ToString());
            }

            if (lineSpan[0] is ';')
            {
                IniComment comment = new(lineSpan[1..].ToString());
                if (currentSectionBuilder is null)
                {
                    builder.Add(comment);
                }
                else
                {
                    currentSectionBuilder.Add(comment);
                }
            }

            if (lineSpan.TrySplitIntoTwo('=', out ReadOnlySpan<char> left, out ReadOnlySpan<char> right))
            {
                IniParameter parameter = new(left.Trim().ToString(), right.Trim().ToString());
                if (currentSectionBuilder is null)
                {
                    builder.Add(parameter);
                }
                else
                {
                    currentSectionBuilder.Add(parameter);
                }
            }
        }

        if (currentSectionBuilder is not null)
        {
            IniSection section = currentSectionBuilder.ToSection();
            builder.Add(section);
            builder.AddRange(section.Children);
        }

        return builder.ToImmutable();
    }

    private static void PrivateSerialize(StreamWriter writer, IEnumerable<IniElement> elements)
    {
        foreach (IniElement element in elements)
        {
            writer.WriteLine(element.ToString());
        }
    }
}