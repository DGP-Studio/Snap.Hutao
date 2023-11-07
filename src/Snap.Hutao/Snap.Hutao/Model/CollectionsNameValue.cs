// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal static class CollectionsNameValue
{
    public static List<NameValue<T>> ListFromEnum<T>()
        where T : struct, Enum
    {
        return Enum.GetValues<T>().Select(x => new NameValue<T>(x.ToString(), x)).ToList();
    }
}