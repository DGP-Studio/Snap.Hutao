// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Response;

internal class ListWrapper<[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)] T>
{
    [JsonPropertyName("list")]
    public ImmutableArray<T> List { get; set; }
}