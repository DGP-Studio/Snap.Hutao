// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Windowing.Abstraction;

internal interface IXamlWindowRectPersisted : IXamlWindowHasInitSize
{
    string PersistRectKey { get; }
}