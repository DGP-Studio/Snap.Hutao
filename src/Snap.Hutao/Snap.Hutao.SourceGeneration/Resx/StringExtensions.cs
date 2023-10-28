using System;
using System.Text;

namespace Snap.Hutao.SourceGeneration.Resx;

internal static class StringExtensions
{
    public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
    {
        StringBuilder sb = new();

        int previousIndex = 0;
        int index = str.IndexOf(oldValue, comparison);
        while (index is not -1)
        {
            sb.Append(str, previousIndex, index - previousIndex);
            sb.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, comparison);
        }

        sb.Append(str, previousIndex, str.Length - previousIndex);
        return sb.ToString();
    }
}