// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using System.Globalization;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class Int32DbProperty : DbProperty<int>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly Func<int> defaultValueFactory;
    private int? field;

    public Int32DbProperty(IServiceProvider serviceProvider, string key, Func<int> defaultValueFactory)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.defaultValueFactory = defaultValueFactory;
    }

    public Int32DbProperty(IServiceProvider serviceProvider, string key, int defaultValue)
        : this(serviceProvider, key, () => defaultValue)
    {
    }

    public override int Value
    {
        get
        {
            if (@field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = GetValue(appDbContext, key);
                    @field = value is null ? defaultValueFactory() : int.Parse(value, CultureInfo.CurrentCulture);
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

    protected override void SetValue(int value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, $"{value}"));
        }
    }
}