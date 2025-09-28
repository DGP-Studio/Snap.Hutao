// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class NullableStringDbProperty : DbProperty<string?>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;

    public NullableStringDbProperty(IServiceProvider serviceProvider, string key)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
    }

    public override string? Value
    {
        get
        {
            if (field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    field = GetValue(appDbContext, key);
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

    protected override void SetValue(string? value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, value));
        }
    }
}