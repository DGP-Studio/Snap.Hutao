// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Abstraction;

internal static class ServiceScopeExtension
{
    public static TService GetRequiredService<TService>(this IServiceScope scope)
        where TService : class
    {
        return scope.ServiceProvider.GetRequiredService<TService>();
    }

    public static TDbContext GetDbContext<TDbContext>(this IServiceScope scope)
        where TDbContext : DbContext
    {
        return scope.GetRequiredService<TDbContext>();
    }

    public static AppDbContext GetAppDbContext(this IServiceScope scope)
    {
        return scope.GetDbContext<AppDbContext>();
    }
}