// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class BooleanDbProperty : DbProperty<bool>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly Func<bool> defaultValueFactory;
    private bool? field;

    public BooleanDbProperty(IServiceProvider serviceProvider, string key, Func<bool> defaultValueFactory)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.defaultValueFactory = defaultValueFactory;
    }

    public BooleanDbProperty(IServiceProvider serviceProvider, string key, bool defaultValue)
        : this(serviceProvider, key, () => defaultValue)
    {
    }

    public override bool Value
    {
        get
        {
            if (@field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = GetValue(appDbContext, key);
                    @field = value is null ? defaultValueFactory() : bool.Parse(value);
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

    protected override void SetValue(bool value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, value ? bool.TrueString : bool.FalseString));
        }
    }
}