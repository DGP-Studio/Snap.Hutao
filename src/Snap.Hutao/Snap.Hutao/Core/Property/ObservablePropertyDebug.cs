// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.Property;

[DebuggerDisplay("{Name,nq} = {Value}")]
internal sealed partial class ObservablePropertyDebug<T> : IObservableProperty<T>
{
    private readonly IObservableProperty<T> source;

    public ObservablePropertyDebug(IObservableProperty<T> source, string name)
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
            Debug.WriteLine($"ObservablePropertyDebug: {Name} get [{result}]\r\n{Environment.StackTrace}");
            return result;
        }

        set
        {
            Debug.WriteLine($"ObservablePropertyDebug: {Name} set [{value}]\r\n{Environment.StackTrace}");
            source.Value = value;
        }
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        return source.GetDeferral();
    }
}