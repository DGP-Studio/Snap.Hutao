// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Text.Json;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class StructUsingJsonDbProperty<T> : StructUsingCustomDbProperty<T>
    where T : struct
{
    public StructUsingJsonDbProperty(IServiceProvider serviceProvider, string key, Func<T> defaultValueFactory)
        : base(serviceProvider, key, defaultValueFactory, From, To)
    {
    }

    public StructUsingJsonDbProperty(IServiceProvider serviceProvider, string key, T defaultValue)
        : this(serviceProvider, key, () => defaultValue)
    {
    }

    private static T From(string source)
    {
        return JsonSerializer.Deserialize<T>(source, JsonOptions.Default);
    }

    private static string To(T value)
    {
        return JsonSerializer.Serialize(value, JsonOptions.Default);
    }
}