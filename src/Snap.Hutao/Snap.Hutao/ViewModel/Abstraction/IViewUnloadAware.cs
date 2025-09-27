// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Property;

namespace Snap.Hutao.ViewModel.Abstraction;

internal interface IViewUnloadAware
{
    IProperty<bool> IsViewUnloaded { get; }
}