// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Reflection.Emit;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup.ILEmit;

internal sealed class ILEmitResolverBuilderContext
{
    public ILEmitResolverBuilderContext(ILGenerator generator)
    {
        Generator = generator;
    }

    public ILGenerator Generator { get; }

    public List<object?>? Constants { get; set; }

    public List<Func<IServiceProvider, object>>? Factories { get; set; }
}