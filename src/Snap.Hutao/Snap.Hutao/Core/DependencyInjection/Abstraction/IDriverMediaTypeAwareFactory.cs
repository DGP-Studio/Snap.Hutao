// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

internal interface IDriverMediaTypeAwareFactory<out TService>
{
    TService Create(string path);
}