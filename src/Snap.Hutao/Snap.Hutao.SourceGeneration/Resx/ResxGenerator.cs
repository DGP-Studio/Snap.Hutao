using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Snap.Hutao.SourceGeneration.Resx;

[Generator]
public sealed class ResxGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor InvalidResx = new("SH401", "Couldn't parse Resx file", "Couldn't parse Resx file '{0}'", "ResxGenerator", DiagnosticSeverity.Warning, true);
    private static readonly DiagnosticDescriptor InvalidPropertiesForNamespace = new("SH402", "Couldn't compute namespace", "Couldn't compute namespace for file '{0}'", "ResxGenerator", DiagnosticSeverity.Warning, true);
    private static readonly DiagnosticDescriptor InvalidPropertiesForResourceName = new("SH403", "Couldn't compute resource name", "Couldn't compute resource name for file '{0}'", "ResxGenerator", DiagnosticSeverity.Warning, true);
    private static readonly DiagnosticDescriptor InconsistentProperties = new("SH404", "Inconsistent properties", "Property '{0}' values for '{1}' are inconsistent", "ResxGenerator", DiagnosticSeverity.Warning, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<(string? AssemblyName, bool SupportNullableReferenceTypes)> compilationProvider = context.CompilationProvider
            .Select(static (compilation, cancellationToken) => (compilation.AssemblyName, SupportNullableReferenceTypes: compilation.GetTypeByMetadataName("System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute") is not null));

        IncrementalValueProvider<ImmutableArray<AdditionalText>> resxProvider = context.AdditionalTextsProvider
            .Where(text => text.Path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
            .Collect();

        context.RegisterSourceOutput(
            source: context.AnalyzerConfigOptionsProvider.Combine(compilationProvider.Combine(resxProvider)),
            action: (ctx, source) => Execute(ctx, source.Left, source.Right.Left.AssemblyName, source.Right.Left.SupportNullableReferenceTypes, source.Right.Right));
    }

    private static void Execute(SourceProductionContext context, AnalyzerConfigOptionsProvider options, string? assemblyName, bool supportNullableReferenceTypes, ImmutableArray<AdditionalText> files)
    {
        // Group additional file by resource kind ((a.resx, a.en.resx, a.en-us.resx), (b.resx, b.en-us.resx))
        List<IGrouping<string, AdditionalText>> resxGroups = files
            .GroupBy(file => GetResourceName(file.Path), StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .ToList();

        foreach (IGrouping<string, AdditionalText>? resxGroug in resxGroups)
        {
            string? rootNamespaceConfiguration = GetMetadataValue(context, options, "RootNamespace", resxGroug);
            string? projectDirConfiguration = GetMetadataValue(context, options, "ProjectDir", resxGroug);
            string? namespaceConfiguration = GetMetadataValue(context, options, "Namespace", "DefaultResourcesNamespace", resxGroug);
            string? resourceNameConfiguration = GetMetadataValue(context, options, "ResourceName", globalName: null, resxGroug);
            string? classNameConfiguration = GetMetadataValue(context, options, "ClassName", globalName: null, resxGroug);

            string rootNamespace = rootNamespaceConfiguration ?? assemblyName ?? "";
            string projectDir = projectDirConfiguration ?? assemblyName ?? "";
            string? defaultResourceName = ComputeResourceName(rootNamespace, projectDir, resxGroug.Key);
            string? defaultNamespace = ComputeNamespace(rootNamespace, projectDir, resxGroug.Key);

            string? ns = namespaceConfiguration ?? defaultNamespace;
            string? resourceName = resourceNameConfiguration ?? defaultResourceName;
            string className = classNameConfiguration ?? ToCSharpNameIdentifier(Path.GetFileName(resxGroug.Key));

            if (ns == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(InvalidPropertiesForNamespace, location: null, resxGroug.First().Path));
            }

            if (resourceName == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(InvalidPropertiesForResourceName, location: null, resxGroug.First().Path));
            }

            List<ResxEntry>? entries = LoadResourceFiles(context, resxGroug);

            string content = $"""
                // Debug info:
                // key: {resxGroug.Key}
                // files: {string.Join(", ", resxGroug.Select(f => f.Path))}
                // RootNamespace (metadata): {rootNamespaceConfiguration}
                // ProjectDir (metadata): {projectDirConfiguration}
                // Namespace / DefaultResourcesNamespace (metadata): {namespaceConfiguration}
                // ResourceName (metadata): {resourceNameConfiguration}
                // ClassName (metadata): {classNameConfiguration}
                // AssemblyName: {assemblyName}
                // RootNamespace (computed): {rootNamespace}
                // ProjectDir (computed): {projectDir}
                // defaultNamespace: {defaultNamespace}
                // defaultResourceName: {defaultResourceName}
                // Namespace: {ns}
                // ResourceName: {resourceName}
                // ClassName: {className}
                """;

            if (resourceName != null && entries != null)
            {
                content += GenerateCode(ns, className, resourceName, entries, supportNullableReferenceTypes);
            }

            context.AddSource($"{Path.GetFileName(resxGroug.Key)}.resx.g.cs", SourceText.From(content, Encoding.UTF8));
        }
    }

    private static string GenerateCode(string? ns, string className, string resourceName, List<ResxEntry> entries, bool enableNullableAttributes)
    {
        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine("#nullable enable");

        if (ns != null)
        {
            sb.AppendLine($$"""

                namespace {{ns}};

                """);
        }

        sb.AppendLine($$"""
            internal partial class {{className}}
            {
                private static global::System.Resources.ResourceManager? resourceMan;

                public {{className}}()
                {
                }

                [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
                public static global::System.Resources.ResourceManager ResourceManager
                {
                    get
                    {
                        if (resourceMan is null) 
                        {
                            resourceMan = new global::System.Resources.ResourceManager("{{resourceName}}", typeof({{className}}).Assembly);
                        }

                        return resourceMan;
                    }
                }

                [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
                public static global::System.Globalization.CultureInfo? Culture { get; set; }

                [return:global::System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("defaultValue")]
                public static object? GetObject(global::System.Globalization.CultureInfo? culture, string name, object? defaultValue)
                {
                    culture ??= Culture;
                    object? obj = ResourceManager.GetObject(name, culture);
                    if (obj == null)
                    {
                        return defaultValue;
                    }
            
                    return obj;
                }

                public static object? GetObject(global::System.Globalization.CultureInfo? culture, string name)
                {
                    return GetObject(culture: culture, name: name, defaultValue: null);
                }
            
                public static object? GetObject(string name)
                {
                    return GetObject(culture: null, name: name, defaultValue: null);
                }

                [return:global::System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("defaultValue")]
                public static object? GetObject(string name, object? defaultValue)
                {
                    return GetObject(culture: null, name: name, defaultValue: defaultValue);
                }
            
                public static global::System.IO.Stream? GetStream(string name)
                {
                    return GetStream(culture: null, name: name);
                }
            
                public static global::System.IO.Stream? GetStream(global::System.Globalization.CultureInfo? culture, string name)
                {
                    culture ??= Culture;
                    return ResourceManager.GetStream(name, culture);
                }
            
                public static string? GetString(global::System.Globalization.CultureInfo? culture, string name)
                {
                    return GetString(culture: culture, name: name, args: null);
                }
            
                public static string? GetString(global::System.Globalization.CultureInfo? culture, string name, params object?[]? args)
                {
                    culture ??= Culture;
                    string? str = ResourceManager.GetString(name, culture);
                    if (str == null)
                    {
                        return null;
                    }
            
                    if (args != null)
                    {
                        return string.Format(culture, str, args);
                    }
                    else
                    {
                        return str;
                    }
                }
                
                public static string? GetString(string name, params object?[]? args)
                {
                    return GetString(culture: null, name: name, args: args);
                }

                [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("defaultValue")]
                public static string? GetString(string name, string? defaultValue)
                {
                    return GetStringOrDefault(culture: null, name: name, defaultValue: defaultValue, args: null);
                }
            
                public static string? GetString(string name)
                {
                    return GetStringOrDefault(culture: null, name: name, defaultValue: null, args: null);
                }

                [return:global::System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("defaultValue")]
                public static string? GetStringOrDefault(global::System.Globalization.CultureInfo? culture, string name, string? defaultValue)
                {
                    return GetStringOrDefault(culture: culture, name: name, defaultValue: defaultValue, args: null);
                }

                [return:global::System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("defaultValue")]
                public static string? GetStringOrDefault(global::System.Globalization.CultureInfo? culture, string name, string? defaultValue, params object?[]? args)
                {
                    culture ??= Culture;
                    string? str = ResourceManager.GetString(name, culture);
                    if (str == null)
                    {
                        if (defaultValue == null || args == null)
                        {
                            return defaultValue;
                        }
                        else
                        {
                            return string.Format(culture, defaultValue, args);
                        }
                    }
            
                    if (args != null)
                    {
                        return string.Format(culture, str, args);
                    }
                    else
                    {
                        return str;
                    }
                }

                [return:global::System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("defaultValue")]
                public static string? GetStringOrDefault(string name, string? defaultValue, params object?[]? args)
                {
                    return GetStringOrDefault(culture: null, name: name, defaultValue: defaultValue, args: args);
                }

                [return:global::System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("defaultValue")]
                public static string? GetStringOrDefault(string name, string? defaultValue)
                {
                    return GetStringOrDefault(culture: null, name: name, defaultValue: defaultValue, args: null);
                }
            """);

        foreach (ResxEntry? entry in entries.OrderBy(e => e.Name, StringComparer.Ordinal))
        {
            if (string.IsNullOrEmpty(entry.Name))
            {
                continue;
            }

            if (entry.IsText)
            {
                XElement summary = new("summary", new XElement("para", $"Looks up a localized string for \"{entry.Name}\"."));
                if (!string.IsNullOrWhiteSpace(entry.Comment))
                {
                    summary.Add(new XElement("para", entry.Comment));
                }

                if (!entry.IsFileRef)
                {
                    summary.Add(new XElement("para", $"Value: \"{entry.Value}\"."));
                }

                string comment = summary.ToString().Replace("\r\n", "\r\n   /// ", StringComparison.Ordinal);

                sb.AppendLine($$"""
                        /// {{comment}}
                        public static string {{ToCSharpNameIdentifier(entry.Name!)}}
                        {
                            get => GetString("{{entry.Name}}")!;
                        }

                    """);

                if (entry.Value != null)
                {
                    int args = Regex.Matches(entry.Value, "\\{(?<num>[0-9]+)(\\:[^}]*)?\\}", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)
                        .Cast<Match>()
                        .Select(m => int.Parse(m.Groups["num"].Value, CultureInfo.InvariantCulture))
                        .Distinct()
                        .DefaultIfEmpty(-1)
                        .Max();

                    if (args >= 0)
                    {
                        string inParams = string.Join(", ", Enumerable.Range(0, args + 1).Select(arg => "object? arg" + arg.ToString(CultureInfo.InvariantCulture)));
                        string callParams = string.Join(", ", Enumerable.Range(0, args + 1).Select(arg => "arg" + arg.ToString(CultureInfo.InvariantCulture)));

                        sb.AppendLine($$"""
                                /// {{comment}}
                                public static string Format{{ToCSharpNameIdentifier(entry.Name!)}}(global::System.Globalization.CultureInfo? provider, {{inParams}})
                                {
                                    return GetString(provider, "{{entry.Name}}", {{callParams}})!;
                                }

                                /// {{comment}}
                                public static string Format{{ToCSharpNameIdentifier(entry.Name!)}}({{inParams}})
                                {
                                    return GetString("{{entry.Name}}", {{callParams}})!;
                                }

                            """);
                    }
                }
            }
            else
            {
                sb.AppendLine($$"""
                        public static global::{{entry.FullTypeName}}? {{ToCSharpNameIdentifier(entry.Name!)}}
                        {
                            get => (global::{{entry.FullTypeName}}?)GetObject("{{entry.Name}}");
                        }

                    """);
            }
        }

        sb.AppendLine($$"""
            }

            internal partial class {{className}}Names
            {
            """);

        foreach (ResxEntry entry in entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
            {
                continue;
            }

            sb.AppendLine($$"""
                    public const string {{ToCSharpNameIdentifier(entry.Name!)}} = "entry.Name";
                """);
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string? ComputeResourceName(string rootNamespace, string projectDir, string resourcePath)
    {
        string fullProjectDir = EnsureEndSeparator(Path.GetFullPath(projectDir));
        string fullResourcePath = Path.GetFullPath(resourcePath);

        if (fullProjectDir == fullResourcePath)
        {
            return rootNamespace;
        }

        if (fullResourcePath.StartsWith(fullProjectDir, StringComparison.Ordinal))
        {
            string relativePath = fullResourcePath.Substring(fullProjectDir.Length);
            return rootNamespace + '.' + relativePath.Replace('/', '.').Replace('\\', '.');
        }

        return null;
    }

    private static string? ComputeNamespace(string rootNamespace, string projectDir, string resourcePath)
    {
        string fullProjectDir = EnsureEndSeparator(Path.GetFullPath(projectDir));
        string fullResourcePath = EnsureEndSeparator(Path.GetDirectoryName(Path.GetFullPath(resourcePath))!);

        if (fullProjectDir == fullResourcePath)
        {
            return rootNamespace;
        }

        if (fullResourcePath.StartsWith(fullProjectDir, StringComparison.Ordinal))
        {
            string relativePath = fullResourcePath.Substring(fullProjectDir.Length);
            return rootNamespace + '.' + relativePath.Replace('/', '.').Replace('\\', '.').TrimEnd('.');
        }

        return null;
    }

    private static List<ResxEntry>? LoadResourceFiles(SourceProductionContext context, IGrouping<string, AdditionalText> resxGroug)
    {
        List<ResxEntry> entries = new();
        foreach (AdditionalText? entry in resxGroug.OrderBy(file => file.Path, StringComparer.Ordinal))
        {
            SourceText? content = entry.GetText(context.CancellationToken);
            if (content == null)
            {
                continue;
            }

            try
            {
                XDocument document = XDocument.Parse(content.ToString());
                foreach (XElement? element in document.XPathSelectElements("/root/data"))
                {
                    string? name = element.Attribute("name")?.Value;
                    string? type = element.Attribute("type")?.Value;
                    string? comment = element.Attribute("comment")?.Value;
                    string? value = element.Element("value")?.Value;

                    ResxEntry existingEntry = entries.Find(e => e.Name == name);
                    if (existingEntry != null)
                    {
                        existingEntry.Comment ??= comment;
                    }
                    else
                    {
                        entries.Add(new ResxEntry { Name = name, Value = value, Comment = comment, Type = type });
                    }
                }
            }
            catch
            {
                context.ReportDiagnostic(Diagnostic.Create(InvalidResx, location: null, entry.Path));
                return null;
            }
        }

        return entries;
    }

    private static string? GetMetadataValue(SourceProductionContext context, AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, string name, IEnumerable<AdditionalText> additionalFiles)
    {
        return GetMetadataValue(context, analyzerConfigOptionsProvider, name, name, additionalFiles);
    }

    private static string? GetMetadataValue(SourceProductionContext context, AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, string name, string? globalName, IEnumerable<AdditionalText> additionalFiles)
    {
        string? result = null;
        foreach (AdditionalText file in additionalFiles)
        {
            if (analyzerConfigOptionsProvider.GetOptions(file).TryGetValue("build_metadata.AdditionalFiles." + name, out string? value))
            {
                if (result != null && value != result)
                {
                    context.ReportDiagnostic(Diagnostic.Create(InconsistentProperties, location: null, name, file.Path));
                    return null;
                }

                result = value;
            }
        }

        if (!string.IsNullOrEmpty(result))
        {
            return result;
        }

        if (globalName != null && analyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property." + globalName, out string? globalValue) && !string.IsNullOrEmpty(globalValue))
        {
            return globalValue;
        }

        return null;
    }

    private static string ToCSharpNameIdentifier(string name)
    {
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#identifiers
        // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.unicodecategory?view=net-5.0
        StringBuilder sb = new();
        foreach (char c in name)
        {
            UnicodeCategory category = char.GetUnicodeCategory(c);
            switch (category)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.LetterNumber:
                    sb.Append(c);
                    break;

                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.Format:
                    if (sb.Length == 0)
                    {
                        sb.Append('_');
                    }
                    sb.Append(c);
                    break;

                default:
                    sb.Append('_');
                    break;
            }
        }

        return sb.ToString();
    }

    private static string EnsureEndSeparator(string path)
    {
        if (path[path.Length - 1] == Path.DirectorySeparatorChar)
        {
            return path;
        }

        return path + Path.DirectorySeparatorChar;
    }

    private static string GetResourceName(string path)
    {
        string pathWithoutExtension = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
        int indexOf = pathWithoutExtension.LastIndexOf('.');
        if (indexOf < 0)
        {
            return pathWithoutExtension;
        }

        return Regex.IsMatch(pathWithoutExtension.Substring(indexOf + 1), "^[a-zA-Z]{2}(-[a-zA-Z]{2})?$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1))
            ? pathWithoutExtension.Substring(0, indexOf)
            : pathWithoutExtension;
    }

    private sealed class ResxEntry
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Comment { get; set; }
        public string? Type { get; set; }

        public bool IsText
        {
            get
            {
                if (Type == null)
                {
                    return true;
                }

                if (Value != null)
                {
                    string[] parts = Value.Split(';');
                    if (parts.Length > 1)
                    {
                        string type = parts[1];
                        if (type.StartsWith("System.String,", StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public string? FullTypeName
        {
            get
            {
                if (IsText)
                {
                    return "string";
                }

                if (Value != null)
                {
                    string[] parts = Value.Split(';');
                    if (parts.Length > 1)
                    {
                        string type = parts[1];
                        return type.Split(',')[0];
                    }
                }

                return null;
            }
        }

        public bool IsFileRef
        {
            get => Type != null && Type.StartsWith("System.Resources.ResXFileRef,", StringComparison.Ordinal);
        }
    }
}
