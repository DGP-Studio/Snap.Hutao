// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace Snap.Hutao.Web.Response;

internal interface ICommonResponse<[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)] out TResponse>
    where TResponse : ICommonResponse<TResponse>
{
    int ReturnCode { get; }

    string Message { get; set; }

    static abstract TResponse CreateDefault(int returnCode, string message);
}