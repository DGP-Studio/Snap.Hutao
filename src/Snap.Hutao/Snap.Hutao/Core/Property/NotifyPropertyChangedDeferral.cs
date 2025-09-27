// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Property;

internal static class NotifyPropertyChangedDeferral
{
    public static NotifyPropertyChangedDeferral<T> Create<T>(T source, Action<T> raisePropertyChanged)
        where T : INotifyPropertyChanged
    {
        return new(source, raisePropertyChanged);
    }

    public static NotifyPropertyChangedDeferral<T, TState> Create<T, TState>(T source, TState state, Action<T, TState> raisePropertyChanged)
        where T : INotifyPropertyChanged
    {
        return new(source, state, raisePropertyChanged);
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class NotifyPropertyChangedDeferral<T> : INotifyPropertyChangedDeferral
    where T : INotifyPropertyChanged
{
    private readonly T source;
    private readonly Action<T> raisePropertyChanged;

    public NotifyPropertyChangedDeferral(T source, Action<T> raisePropertyChanged)
    {
        this.source = source;
        this.raisePropertyChanged = raisePropertyChanged;
    }

    public void Dispose()
    {
        raisePropertyChanged(source);
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class NotifyPropertyChangedDeferral<T, TState> : INotifyPropertyChangedDeferral
    where T : INotifyPropertyChanged
{
    private readonly T source;
    private readonly TState state;
    private readonly Action<T, TState> raisePropertyChanged;

    public NotifyPropertyChangedDeferral(T source, TState state, Action<T, TState> raisePropertyChanged)
    {
        this.source = source;
        this.raisePropertyChanged = raisePropertyChanged;
        this.state = state;
    }

    public void Dispose()
    {
        raisePropertyChanged(source, state);
    }
}