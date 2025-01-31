// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace Snap.Hutao.Web.Response;

internal sealed class DataWrapper<[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)] T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}