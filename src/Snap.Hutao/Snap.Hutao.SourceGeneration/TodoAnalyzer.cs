// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Snap.Hutao.SourceGeneration;

/// <summary>
/// 高亮TODO
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class TodoAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Descriptor =
        new("SH0001", "TODO 项尚未实现", "此 TODO 项需要实现", "Standard", DiagnosticSeverity.Info, true);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get => ImmutableArray.Create(Descriptor); }

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxTreeAction(HandleSyntaxTree);
    }

    private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
        foreach (SyntaxTrivia node in root.DescendantTrivia(descendIntoTrivia: true))
        {
            switch (node.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    string text = node.ToString().ToLowerInvariant();
                    if (text.Contains("todo:"))
                    {
                        string hint = node.ToString().Substring(text.IndexOf("todo:") + 6);
                        DiagnosticDescriptor descriptor = new("SH0001", "TODO 项尚未实现", hint, "Standard", DiagnosticSeverity.Info, true);
                        context.ReportDiagnostic(Diagnostic.Create(descriptor, node.GetLocation()));
                    }
                    break;
            }
        }
    }
}
