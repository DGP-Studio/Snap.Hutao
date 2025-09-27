// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class StringDbProperty : DbProperty<string>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly Func<string> defaultValueFactory;

    public StringDbProperty(IServiceProvider serviceProvider, string key, Func<string> defaultValueFactory)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.defaultValueFactory = defaultValueFactory;
    }

    public StringDbProperty(IServiceProvider serviceProvider, string key, string defaultValue)
        : this(serviceProvider, key, () => defaultValue)
    {
    }

    [field: MaybeNull]
    public override string Value
    {
        get
        {
            if (field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    field = GetValue(appDbContext, key) ?? defaultValueFactory();
                }
            }

            return field;
        }

        set
        {
            if (Volatile.Read(ref Deferring))
            {
                field = value;
                SetValue(value);
            }
            else
            {
                if (SetProperty(ref field, value))
                {
                    SetValue(value);
                }
            }
        }
    }

    protected override void SetValue(string value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, value));
        }
    }
}