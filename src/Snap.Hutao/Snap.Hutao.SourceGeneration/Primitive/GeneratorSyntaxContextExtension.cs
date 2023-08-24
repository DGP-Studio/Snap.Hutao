// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics.CodeAnalysis;

namespace Snap.Hutao.SourceGeneration.Primitive;

internal static class GeneratorSyntaxContextExtension
{
    public static bool TryGetDeclaredSymbol<TSymbol>(this GeneratorSyntaxContext context, System.Threading.CancellationToken token, [NotNullWhen(true)] out TSymbol? symbol)
        where TSymbol : class, ISymbol
    {
        symbol = context.SemanticModel.GetDeclaredSymbol(context.Node, token) as TSymbol;
        return symbol != null;
    }
}