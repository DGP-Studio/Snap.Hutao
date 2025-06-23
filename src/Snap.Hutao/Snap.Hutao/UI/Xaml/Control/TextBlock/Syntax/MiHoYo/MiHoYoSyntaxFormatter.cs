// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Collections.Immutable;
using System.Text;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal static class MiHoYoSyntaxFormatter
{
    public static string Format(ImmutableArray<MiHoYoSyntaxElement> elements, string input)
    {
        StringBuilder stringBuilder = new();
        Format(elements, input, indentLevel: 0, stringBuilder);
        return stringBuilder.ToString();
    }

    private static void Format(ImmutableArray<MiHoYoSyntaxElement> elements, string input, int indentLevel, StringBuilder stringBuilder)
    {
        foreach (ref readonly MiHoYoSyntaxElement element in elements.AsSpan())
        {
            FormatElement(element, input, indentLevel, stringBuilder);
        }
    }

    private static void FormatElement(MiHoYoSyntaxElement node, string input, int indent, StringBuilder stringBuilder)
    {
        string indentString = new(' ', indent * 2);
        string content = input.Substring(node.Position.Start, node.Position.Length);

        switch (node)
        {
            case MiHoYoSyntaxTextElement:
                stringBuilder.AppendLine($"{indentString}Text: \"{content}\"");
                break;

            case MiHoYoSyntaxItalicElement italic:
                stringBuilder.AppendLine($"{indentString}Italic:");
                Format(italic.Children, input, indent + 1, stringBuilder);
                break;

            case MiHoYoSyntaxColorElement color:
                stringBuilder.AppendLine($"{indentString}Color:");
                Format(color.Children, input, indent + 1, stringBuilder);
                break;

            case MiHoYoSyntaxLinkElement link:
                stringBuilder.AppendLine($"{indentString}Link:");
                Format(link.Children, input, indent + 1, stringBuilder);
                break;

            default:
                stringBuilder.AppendLine($"{indentString}Unknown element: {node.GetType().Name}");
                break;
        }
    }
}