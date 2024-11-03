// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Windowing.Abstraction;

internal interface IXamlWindowRectPersisted : IXamlWindowHasInitSize
{
    string PersistRectKey { get; }

    string PersistScaleKey { get; }
}