// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Diagnostics;

namespace Snap.Hutao.Core.Property;

[DebuggerDisplay("{Name,nq} = {Value}")]
internal sealed partial class ReadOnlyObservablePropertyDebug<T> : IReadOnlyObservableProperty<T>
{
    private readonly IReadOnlyObservableProperty<T> source;

    public ReadOnlyObservablePropertyDebug(IReadOnlyObservableProperty<T> source, string name)
    {
        this.source = source;
        Name = name;
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => source.PropertyChanged += value;
        remove => source.PropertyChanged -= value;
    }

    public string Name { get; }

    public T Value
    {
        get
        {
            T result = source.Value;
            Debug.WriteLine($"ReadOnlyObservablePropertyDebug: {Name} get [{result}]\r\n{Environment.StackTrace}");
            return result;
        }
    }
}