// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace Snap.Hutao.SourceGeneration.Primitive;

internal readonly struct GeneratorSyntaxContext2
{
    public readonly GeneratorSyntaxContext Context;
    public readonly INamedTypeSymbol Symbol;
    public readonly ImmutableArray<AttributeData> Attributes;
    public readonly bool HasValue = false;

    public GeneratorSyntaxContext2(GeneratorSyntaxContext context, INamedTypeSymbol symbol, ImmutableArray<AttributeData> attributes)
    {
        Context = context;
        Symbol = symbol;
        Attributes = attributes;
        HasValue = true;
    }

    public static bool NotNull(GeneratorSyntaxContext2 context)
    {
        return context.HasValue;
    }

    public bool HasAttributeWithName(string name)
    {
        return Attributes.Any(attr => attr.AttributeClass!.ToDisplayString() == name);
    }

    public AttributeData SingleAttributeWithName(string name)
    {
        return Attributes.Single(attribute => attribute.AttributeClass!.ToDisplayString() == name);
    }

    public AttributeData? SingleOrDefaultAttributeWithName(string name)
    {
        return Attributes.SingleOrDefault(attribute => attribute.AttributeClass!.ToDisplayString() == name);
    }
}