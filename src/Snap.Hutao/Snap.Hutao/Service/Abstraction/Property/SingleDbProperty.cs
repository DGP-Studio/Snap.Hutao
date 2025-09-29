// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using System.Globalization;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class SingleDbProperty : DbProperty<float>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly Func<float> defaultValueFactory;
    private float? field;

    public SingleDbProperty(IServiceProvider serviceProvider, string key, Func<float> defaultValueFactory)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.defaultValueFactory = defaultValueFactory;
    }

    public SingleDbProperty(IServiceProvider serviceProvider, string key, float defaultValue)
        : this(serviceProvider, key, () => defaultValue)
    {
    }

    public override float Value
    {
        get
        {
            if (@field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = GetValue(appDbContext, key);
                    @field = value is null ? defaultValueFactory() : float.Parse(value, CultureInfo.CurrentCulture);
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

    protected override void SetValue(float value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, $"{value}"));
        }
    }
}