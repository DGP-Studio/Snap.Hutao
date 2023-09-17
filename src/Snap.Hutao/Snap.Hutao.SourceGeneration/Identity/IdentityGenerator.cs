using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace Snap.Hutao.SourceGeneration.Identity;

[Generator(LanguageNames.CSharp)]
internal sealed class IdentityGenerator : IIncrementalGenerator
{
    private const string FileName = "IdentityStructs.json";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<AdditionalText>> provider = context.AdditionalTextsProvider.Where(MatchFileName).Collect();

        context.RegisterImplementationSourceOutput(provider, GenerateIdentityStructs);
    }

    private static bool MatchFileName(AdditionalText text)
    {
        return Path.GetFileName(text.Path) == FileName;
    }

    private static void GenerateIdentityStructs(SourceProductionContext context, ImmutableArray<AdditionalText> texts)
    {
        AdditionalText jsonFile = texts.Single();

        string identityJson = jsonFile.GetText(context.CancellationToken)!.ToString();
        List<IdentityStructMetadata> identities = identityJson.FromJson<List<IdentityStructMetadata>>()!;

        if (identities.Any())
        {
            foreach (IdentityStructMetadata identityStruct in identities)
            {
                GenerateIdentityStruct(context, identityStruct);
            }
        }
    }

    private static void GenerateIdentityStruct(SourceProductionContext context, IdentityStructMetadata metadata)
    {
        string name = metadata.Name;

        StringBuilder sourceBuilder = new StringBuilder().AppendLine($$"""
            // Copyright (c) DGP Studio. All rights reserved.
            // Licensed under the MIT license.

            using Snap.Hutao.Model.Primitive.Converter;
            using System.Numerics;

            namespace Snap.Hutao.Model.Primitive;

            /// <summary>
            /// {{metadata.Documentation}}
            /// </summary>
            [JsonConverter(typeof(IdentityConverter<{{name}}>))]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(IdentityGenerator)}}","1.0.0.0")]
            internal readonly partial struct {{name}}
            {
                /// <summary>
                /// 值
                /// </summary>
                public readonly uint Value;

                /// <summary>
                /// Initializes a new instance of the <see cref="{{name}}"/> struct.
                /// </summary>
                /// <param name="value">value</param>
                public {{name}}(uint value)
                {
                    Value = value;
                }

                public static implicit operator uint({{name}} value)
                {
                    return value.Value;
                }

                public static implicit operator {{name}}(uint value)
                {
                    return new(value);
                }

                /// <inheritdoc/>
                public override int GetHashCode()
                {
                    return Value.GetHashCode();
                }

                /// <inheritdoc/>
                public override string ToString()
                {
                    return Value.ToString();
                }
            }
            """);

        if (metadata.Equatable)
        {
            sourceBuilder.AppendLine($$"""

                internal readonly partial struct {{name}} : IEquatable<{{name}}>
                {
                    /// <inheritdoc/>
                    public override bool Equals(object obj)
                    {
                        return obj is {{name}} other && Equals(other);
                    }

                    /// <inheritdoc/>
                    public bool Equals({{name}} other)
                    {
                        return Value == other.Value;
                    }
                }
                """);
        }

        if (metadata.EqualityOperators)
        {
            sourceBuilder.AppendLine($$"""

                internal readonly partial struct {{name}} : IEqualityOperators<{{name}}, {{name}}, bool>, IEqualityOperators<{{name}}, uint, bool>
                {
                    public static bool operator ==({{name}} left, {{name}} right)
                    {
                        return left.Value == right.Value;
                    }
                
                    public static bool operator ==({{name}} left, uint right)
                    {
                        return left.Value == right;
                    }

                    public static bool operator !=({{name}} left, {{name}} right)
                    {
                        return !(left == right);
                    }

                    public static bool operator !=({{name}} left, uint right)
                    {
                        return !(left == right);
                    }
                }
                """);
        }

        if (metadata.AdditionOperators)
        {
            sourceBuilder.AppendLine($$"""

                internal readonly partial struct {{name}} : IAdditionOperators<{{name}}, {{name}}, {{name}}>, IAdditionOperators<{{name}}, uint, {{name}}>
                {
                    public static {{name}} operator +({{name}} left, {{name}} right)
                    {
                        return left.Value + right.Value;
                    }

                    public static {{name}} operator +({{name}} left, uint right)
                    {
                        return left.Value + right;
                    }
                }
                """);
        }

        if (metadata.IncrementOperators)
        {
            sourceBuilder.AppendLine($$"""

                internal readonly partial struct {{name}} : IIncrementOperators<{{name}}>
                {
                    public static unsafe {{name}} operator ++({{name}} value)
                    {
                        ++*(uint*)&value;
                        return value;
                    }
                }
                """);
        }

        context.AddSource($"{name}.g.cs", sourceBuilder.ToString());
    }

    private sealed class IdentityStructMetadata
    {
        public string Name { get; set; } = default!;

        public string? Documentation { get; set; }

        public bool Equatable { get; set; }

        public bool EqualityOperators { get; set; }

        public bool AdditionOperators { get; set; }

        public bool IncrementOperators { get; set; }
    }
}