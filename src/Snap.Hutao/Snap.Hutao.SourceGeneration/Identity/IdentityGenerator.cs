using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

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
            GenerateIdentityConverter(context);

            foreach (IdentityStructMetadata identityStruct in identities)
            {
                GenerateIdentityStruct(context, identityStruct);
            }
        }
    }

    private static void GenerateIdentityConverter(SourceProductionContext context)
    {
        string source = $$"""
            // Copyright (c) DGP Studio. All rights reserved.
            // Licensed under the MIT license.

            namespace Snap.Hutao.Model.Primitive.Converter;

            /// <summary>
            /// Id 转换器
            /// </summary>
            /// <typeparam name="TWrapper">包装类型</typeparam>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(IdentityGenerator)}}","1.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            internal unsafe sealed class IdentityConverter<TWrapper> : JsonConverter<TWrapper>
                where TWrapper : unmanaged
            {
                /// <inheritdoc/>
                public override TWrapper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                {
                    if (reader.TokenType == JsonTokenType.Number)
                    {
                        int value = reader.GetInt32();
                        return *(TWrapper*)&value;
                    }

                    throw new JsonException();
                }

                /// <inheritdoc/>
                public override void Write(Utf8JsonWriter writer, TWrapper value, JsonSerializerOptions options)
                {
                    writer.WriteNumberValue(*(int*)&value);
                }
            }
            """;

        context.AddSource("IdentityConverter.g.cs", source);
    }

    private static void GenerateIdentityStruct(SourceProductionContext context, IdentityStructMetadata identityStruct)
    {
        string name = identityStruct.Name;
        string type = identityStruct.Type;

        string source = $$"""
            // Copyright (c) DGP Studio. All rights reserved.
            // Licensed under the MIT license.

            using Snap.Hutao.Model.Primitive.Converter;

            namespace Snap.Hutao.Model.Primitive;

            /// <summary>
            /// {{identityStruct.Documentation}}
            /// </summary>
            [JsonConverter(typeof(IdentityConverter<{{name}}>))]
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(IdentityGenerator)}}","1.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            internal readonly struct {{name}} : IEquatable<{{name}}>
            {
                /// <summary>
                /// 值
                /// </summary>
                public readonly {{type}} Value;

                /// <summary>
                /// Initializes a new instance of the <see cref="{{name}}"/> struct.
                /// </summary>
                /// <param name="value">value</param>
                public {{name}}({{type}} value)
                {
                    Value = value;
                }

                public static implicit operator {{type}}({{name}} value)
                {
                    return value.Value;
                }

                public static implicit operator {{name}}({{type}} value)
                {
                    return new(value);
                }

                public static bool operator ==({{name}} left, {{name}} right)
                {
                    return left.Value == right.Value;
                }

                public static bool operator !=({{name}} left, {{name}} right)
                {
                    return !(left == right);
                }

                /// <inheritdoc/>
                public bool Equals({{name}} other)
                {
                    return Value == other.Value;
                }

                /// <inheritdoc/>
                public override bool Equals(object obj)
                {
                    return obj is {{name}} other && Equals(other);
                }

                /// <inheritdoc/>
                public override int GetHashCode()
                {
                    return Value.GetHashCode();
                }
            }
            """;

        context.AddSource($"{name}.g.cs", source);
    }

    private sealed class IdentityStructMetadata
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 基底类型
        /// </summary>
        public string Type { get; set; } = default!;

        /// <summary>
        /// 文档
        /// </summary>
        public string? Documentation { get; set; }
    }
}