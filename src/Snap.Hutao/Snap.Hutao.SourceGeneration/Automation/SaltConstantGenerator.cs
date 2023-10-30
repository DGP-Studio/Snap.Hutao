// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Snap.Hutao.SourceGeneration.Automation;

[Generator(LanguageNames.CSharp)]
internal sealed class SaltConstantGenerator : IIncrementalGenerator
{
    private static readonly HttpClient httpClient;
    private static readonly Lazy<Response<SaltLatest>> lazySaltInfo;

    static SaltConstantGenerator()
    {
        httpClient = new();
        lazySaltInfo = new Lazy<Response<SaltLatest>>(() =>
        {
            string body = httpClient.GetStringAsync("https://internal.snapgenshin.cn/Archive/Salt/Latest").GetAwaiter().GetResult();
            return JsonParser.FromJson<Response<SaltLatest>>(body)!;
        });
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateSaltContstants);
    }

    private static void GenerateSaltContstants(IncrementalGeneratorPostInitializationContext context)
    {
        Response<SaltLatest> saltInfo = lazySaltInfo.Value;
        string code = $$"""
            namespace Snap.Hutao.Web.Hoyolab;

            internal sealed class SaltConstants
            {
                public const string CNVersion = "{{saltInfo.Data.CNVersion}}";
                public const string CNK2 = "{{saltInfo.Data.CNK2}}";
                public const string CNLK2 = "{{saltInfo.Data.CNLK2}}";

                public const string OSVersion = "{{saltInfo.Data.OSVersion}}";
                public const string OSK2 = "{{saltInfo.Data.OSK2}}";
                public const string OSLK2 = "{{saltInfo.Data.OSLK2}}";
            }
            """;
        context.AddSource("SaltConstants.g.cs", code);
    }

    private sealed class Response<T>
    {
        [DataMember(Name = "data")]
        public T Data { get; set; } = default!;
    }

    internal sealed class SaltLatest
    {
        public string CNVersion { get; set; } = default!;

        public string CNK2 { get; set; } = default!;

        public string CNLK2 { get; set; } = default!;

        public string OSVersion { get; set; } = default!;

        public string OSK2 { get; set; } = default!;

        public string OSLK2 { get; set; } = default!;
    }
}