// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Primitives;
using Windows.Foundation.Metadata;

namespace Snap.Hutao.View.Helper;

internal sealed class LoadDeferral : ObservableObject
{
    private bool canLoad;

    public bool CanLoad { get => canLoad; set => SetProperty(ref canLoad, value); }
}