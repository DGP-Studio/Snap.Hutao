// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class StructToJsonDbProperty<T> : DbProperty<T>
    where T : struct
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly Func<T> defaultValueFactory;
    private T? field;

    public StructToJsonDbProperty(IServiceProvider serviceProvider, string key, Func<T> defaultValueFactory)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.defaultValueFactory = defaultValueFactory;
    }

    public StructToJsonDbProperty(IServiceProvider serviceProvider, string key, T defaultValue)
        : this(serviceProvider, key, () => defaultValue)
    {
    }

    public override T Value
    {
        get
        {
            if (@field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = GetValue(appDbContext, key);
                    @field = string.IsNullOrEmpty(value)
                        ? defaultValueFactory()
                        : JsonSerializer.Deserialize<T>(value, JsonOptions.Default);
                }
            }

            return @field.Value;
        }

        set
        {
            if (SetProperty(ref @field, value))
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
                    appDbContext.Settings.AddAndSave(new(key, JsonSerializer.Serialize(value, JsonOptions.Default)));
                }
            }
        }
    }
}