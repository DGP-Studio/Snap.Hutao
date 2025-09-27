// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Property;

internal interface IObservableProperty<T> : IProperty<T>, INotifyPropertyChanged
{
    INotifyPropertyChangedDeferral GetDeferral();
}