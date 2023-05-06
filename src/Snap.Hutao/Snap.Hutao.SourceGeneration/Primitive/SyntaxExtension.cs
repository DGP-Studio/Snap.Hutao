// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Snap.Hutao.SourceGeneration.Primitive;

internal static class SyntaxExtension
{
    /// <summary>
    /// Checks whether a given <see cref="MemberDeclarationSyntax"/> has or could potentially have any attribute lists.
    /// </summary>
    /// <param name="declaration">The input <see cref="MemberDeclarationSyntax"/> to check.</param>
    /// <returns>Whether <paramref name="declaration"/> has or potentially has any attribute lists.</returns>
    public static bool HasAttributeLists<TSyntax>(this TSyntax declaration)
        where TSyntax : MemberDeclarationSyntax
    {
        return declaration.AttributeLists.Count > 0;
    }
}