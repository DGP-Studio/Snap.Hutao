// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal static class CollectionsNameValue
{
    public static List<NameValue<TEnum>> FromEnum<TEnum>()
        where TEnum : struct, Enum
    {
        return [.. Enum.GetValues<TEnum>().Select(x => new NameValue<TEnum>(x.ToString(), x))];
    }

    public static List<NameValue<TEnum>> FromEnum<TEnum>(Func<TEnum, bool> condition)
        where TEnum : struct, Enum
    {
        return [.. Enum.GetValues<TEnum>().Where(condition).Select(x => new NameValue<TEnum>(x.ToString(), x))];
    }

    public static List<NameValue<TEnum>> FromEnum<TEnum>(Func<TEnum, string> nameSelector)
        where TEnum : struct, Enum
    {
        return [.. Enum.GetValues<TEnum>().Select(x => new NameValue<TEnum>(nameSelector(x), x))];
    }

    public static List<NameValue<TSource>> From<TSource>(IEnumerable<TSource> sources, Func<TSource, string> nameSelector)
    {
        return [.. sources.Select(x => new NameValue<TSource>(nameSelector(x), x))];
    }
}