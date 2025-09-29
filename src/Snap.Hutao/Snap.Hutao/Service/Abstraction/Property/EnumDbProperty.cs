// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class EnumDbProperty<TEnum> : DbProperty<TEnum>
    where TEnum : struct, Enum
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly Func<TEnum> defaultValueFactory;
    private TEnum? field;

    public EnumDbProperty(IServiceProvider serviceProvider, string key, Func<TEnum> defaultValueFactory)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.defaultValueFactory = defaultValueFactory;
    }

    public EnumDbProperty(IServiceProvider serviceProvider, string key, TEnum defaultValue)
        : this(serviceProvider, key, () => defaultValue)
    {
    }

    public override TEnum Value
    {
        get
        {
            if (@field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = GetValue(appDbContext, key);
                    @field = value is null ? defaultValueFactory() : Enum.Parse<TEnum>(value);
                }
            }

            return @field.Value;
        }

        set
        {
            if (Volatile.Read(ref Deferring))
            {
                @field = value;
                SetValue(value);
            }
            else
            {
                if (SetProperty(ref @field, value))
                {
                    SetValue(value);
                }
            }
        }
    }

    protected override void SetValue(TEnum value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, $"{value}"));
        }
    }
}