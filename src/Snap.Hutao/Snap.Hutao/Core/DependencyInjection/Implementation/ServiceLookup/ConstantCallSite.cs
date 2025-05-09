// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class ConstantCallSite : ServiceCallSite
{
    public ConstantCallSite(Type serviceType, object? defaultValue, object? serviceKey = null)
        : base(ResultCache.None(serviceType), serviceKey)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        if (defaultValue != null && !serviceType.IsInstanceOfType(defaultValue))
        {
            throw new ArgumentException($"Constant value of type '{defaultValue.GetType()}' can't be converted to service type '{serviceType}'");
        }

        Value = defaultValue;
    }

    public override Type ServiceType { get; }

    public override Type ImplementationType { get => DefaultValue?.GetType() ?? ServiceType; }

    public override CallSiteKind Kind { get; } = CallSiteKind.Constant;

    internal object? DefaultValue { get => Value; }
}