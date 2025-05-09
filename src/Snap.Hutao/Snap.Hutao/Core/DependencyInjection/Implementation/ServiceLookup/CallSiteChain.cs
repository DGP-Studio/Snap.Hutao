// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class CallSiteChain
{
    private readonly Dictionary<ServiceIdentifier, ChainItemInfo> callSiteChain;

    public CallSiteChain()
    {
        callSiteChain = [];
    }

    public void CheckCircularDependency(ServiceIdentifier serviceIdentifier)
    {
        if (callSiteChain.ContainsKey(serviceIdentifier))
        {
            throw new InvalidOperationException(CreateCircularDependencyExceptionMessage(serviceIdentifier));
        }
    }

    public void Remove(ServiceIdentifier serviceIdentifier)
    {
        callSiteChain.Remove(serviceIdentifier);
    }

    public void Add(ServiceIdentifier serviceIdentifier, Type? implementationType = null)
    {
        callSiteChain[serviceIdentifier] = new(callSiteChain.Count, implementationType);
    }

    private string CreateCircularDependencyExceptionMessage(ServiceIdentifier serviceIdentifier)
    {
        StringBuilder messageBuilder = new();
        messageBuilder.Append($"A circular dependency was detected for the service of type '{TypeNameHelper.GetTypeDisplayName(serviceIdentifier.ServiceType)}'.");
        messageBuilder.AppendLine();

        AppendResolutionPath(messageBuilder, serviceIdentifier);

        return messageBuilder.ToString();
    }

    private void AppendResolutionPath(StringBuilder builder, ServiceIdentifier currentlyResolving)
    {
        List<KeyValuePair<ServiceIdentifier, ChainItemInfo>> ordered = new(callSiteChain);
        ordered.Sort((a, b) => a.Value.Order.CompareTo(b.Value.Order));

        foreach ((ServiceIdentifier serviceIdentifier, ChainItemInfo value) in ordered)
        {
            Type? implementationType = value.ImplementationType;
            if (implementationType == null || serviceIdentifier.ServiceType == implementationType)
            {
                builder.Append(TypeNameHelper.GetTypeDisplayName(serviceIdentifier.ServiceType));
            }
            else
            {
                builder.Append(TypeNameHelper.GetTypeDisplayName(serviceIdentifier.ServiceType))
                    .Append('(')
                    .Append(TypeNameHelper.GetTypeDisplayName(implementationType))
                    .Append(')');
            }

            builder.Append(" -> ");
        }

        builder.Append(TypeNameHelper.GetTypeDisplayName(currentlyResolving.ServiceType));
    }

    private readonly struct ChainItemInfo
    {
        public ChainItemInfo(int order, Type? implementationType)
        {
            Order = order;
            ImplementationType = implementationType;
        }

        public int Order { get; }

        public Type? ImplementationType { get; }
    }
}