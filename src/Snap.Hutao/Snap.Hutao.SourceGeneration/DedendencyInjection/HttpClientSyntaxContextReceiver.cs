// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.SourceGeneration.DedendencyInjection;

/// <summary>
/// Created on demand before each generation pass
/// </summary>
public class HttpClientSyntaxContextReceiver : ISyntaxContextReceiver
{
    /// <summary>
    /// 注入特性的名称
    /// </summary>
    public const string AttributeName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientAttribute";

    /// <summary>
    /// 所有需要注入的类型符号
    /// </summary>
    public List<INamedTypeSymbol> Classes { get; } = new();

    /// <inheritdoc/>
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        // any class with at least one attribute is a candidate for injection generation
        if (context.Node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0)
        {
            // get as named type symbol
            if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is INamedTypeSymbol classSymbol)
            {
                if (classSymbol.GetAttributes().Any(ad => ad.AttributeClass!.ToDisplayString() == AttributeName))
                {
                    Classes.Add(classSymbol);
                }
            }
        }
    }
}