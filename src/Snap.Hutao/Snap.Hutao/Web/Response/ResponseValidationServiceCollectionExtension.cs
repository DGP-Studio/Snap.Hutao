// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

internal static class ResponseValidationServiceCollectionExtension
{
    public static IServiceCollection AddResponseValidation(this IServiceCollection services)
    {
        return services
            .AddTransient<ICommonResponseValidator<Response>, DefaultResponseValidator>()
            .AddTransient(typeof(ITypedResponseValidator<>), typeof(TypedResponseValidator<>));
    }
}