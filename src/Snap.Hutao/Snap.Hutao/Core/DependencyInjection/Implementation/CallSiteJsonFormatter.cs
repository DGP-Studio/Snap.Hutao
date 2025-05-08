// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;
using System.Text;

namespace Snap.Hutao.Core.DependencyInjection.Implementation;

internal sealed class CallSiteJsonFormatter : CallSiteVisitor<CallSiteJsonFormatter.CallSiteFormatterContext, object?>
{
    internal static readonly CallSiteJsonFormatter Instance = new();

    private CallSiteJsonFormatter()
    {
    }

    public string Format(ServiceCallSite callSite)
    {
        StringBuilder stringBuilder = new();
        CallSiteFormatterContext context = new(stringBuilder, 0, []);

        VisitCallSite(callSite, context);

        return stringBuilder.ToString();
    }

    protected override object? VisitConstructor(ConstructorCallSite constructorCallSite, CallSiteFormatterContext argument)
    {
        argument.WriteProperty("implementationType", constructorCallSite.ImplementationType);

        if (constructorCallSite.ParameterCallSites.Length > 0)
        {
            argument.StartProperty("arguments");

            CallSiteFormatterContext childContext = argument.StartArray();
            foreach (ServiceCallSite parameter in constructorCallSite.ParameterCallSites)
            {
                childContext.StartArrayItem();
                VisitCallSite(parameter, childContext);
            }

            argument.EndArray();
        }

        return null;
    }

    protected override object? VisitCallSiteMain(ServiceCallSite callSite, CallSiteFormatterContext argument)
    {
        if (argument.ShouldFormat(callSite))
        {
            CallSiteFormatterContext childContext = argument.StartObject();

            childContext.WriteProperty("serviceType", callSite.ServiceType);
            childContext.WriteProperty("kind", callSite.Kind);
            childContext.WriteProperty("cache", callSite.Cache.Location);

            base.VisitCallSiteMain(callSite, childContext);

            argument.EndObject();
        }
        else
        {
            CallSiteFormatterContext childContext = argument.StartObject();
            childContext.WriteProperty("ref", callSite.ServiceType);
            argument.EndObject();
        }

        return null;
    }

    protected override object? VisitConstant(ConstantCallSite constantCallSite, CallSiteFormatterContext argument)
    {
        argument.WriteProperty("value", constantCallSite.DefaultValue ?? string.Empty);

        return null;
    }

    protected override object? VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, CallSiteFormatterContext argument)
    {
        return null;
    }

    protected override object? VisitIEnumerable(IEnumerableCallSite enumerableCallSite, CallSiteFormatterContext argument)
    {
        argument.WriteProperty("itemType", enumerableCallSite.ItemType);
        argument.WriteProperty("size", enumerableCallSite.ServiceCallSites.Length);

        if (enumerableCallSite.ServiceCallSites.Length > 0)
        {
            argument.StartProperty("items");

            CallSiteFormatterContext childContext = argument.StartArray();
            foreach (ServiceCallSite item in enumerableCallSite.ServiceCallSites)
            {
                childContext.StartArrayItem();
                VisitCallSite(item, childContext);
            }

            argument.EndArray();
        }

        return null;
    }

    protected override object? VisitFactory(FactoryCallSite factoryCallSite, CallSiteFormatterContext argument)
    {
        argument.WriteProperty("method", factoryCallSite.Factory.Method);

        return null;
    }

    internal struct CallSiteFormatterContext
    {
        private readonly HashSet<ServiceCallSite> processedCallSites;
        private bool firstItem;

        public CallSiteFormatterContext(StringBuilder builder, int offset, HashSet<ServiceCallSite> processedCallSites)
        {
            Builder = builder;
            Offset = offset;
            this.processedCallSites = processedCallSites;
            firstItem = true;
        }

        public int Offset { get; }

        public StringBuilder Builder { get; }

        public bool ShouldFormat(ServiceCallSite serviceCallSite)
        {
            return processedCallSites.Add(serviceCallSite);
        }

        public CallSiteFormatterContext IncrementOffset()
        {
            return new CallSiteFormatterContext(Builder, Offset + 4, processedCallSites)
            {
                firstItem = true,
            };
        }

        public CallSiteFormatterContext StartObject()
        {
            Builder.Append('{');
            return IncrementOffset();
        }

        public void EndObject()
        {
            Builder.Append('}');
        }

        public void StartProperty(string name)
        {
            if (!firstItem)
            {
                Builder.Append(',');
            }
            else
            {
                firstItem = false;
            }

            Builder.Append('"').Append(name).Append("\":");
        }

        public void StartArrayItem()
        {
            if (!firstItem)
            {
                Builder.Append(',');
            }
            else
            {
                firstItem = false;
            }
        }

        public void WriteProperty(string name, object? value)
        {
            StartProperty(name);
            if (value != null)
            {
                Builder.Append(" \"").Append(value).Append('"');
            }
            else
            {
                Builder.Append("null");
            }
        }

        public CallSiteFormatterContext StartArray()
        {
            Builder.Append('[');
            return IncrementOffset();
        }

        public void EndArray()
        {
            Builder.Append(']');
        }
    }
}