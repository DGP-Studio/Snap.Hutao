// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal static class CollectionsNameValue
{
    public static List<NameValue<TEnum>> ListFromEnum<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>().Select(x => new NameValue<TEnum>(x.ToString(), x)).ToList();
    }

    public static List<NameValue<TEnum>> ListFromEnum<TEnum>(Func<TEnum, string> nameSelector)
        where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>().Select(x => new NameValue<TEnum>(nameSelector(x), x)).ToList();
    }
}