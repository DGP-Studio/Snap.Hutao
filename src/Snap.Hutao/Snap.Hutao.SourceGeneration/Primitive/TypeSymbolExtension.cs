// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace Snap.Hutao.SourceGeneration.Primitive;

internal static class TypeSymbolExtension
{
    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> is or inherits from <paramref name="name"/>.</returns>
    public static bool IsOrInheritsFrom(this ITypeSymbol typeSymbol, string name)
    {
        for (ITypeSymbol? currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.ToDisplayString() == name)
            {
                return true;
            }
        }

        return false;
    }
}