// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.Abstraction.Property;

internal partial class ClassUsingCustomDbProperty<T> : DbProperty<T>
    where T : class
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly Func<T> defaultValueFactory;
    private readonly Func<string, T> from;
    private readonly Func<T, string> to;

    public ClassUsingCustomDbProperty(IServiceProvider serviceProvider, string key, Func<T> defaultValueFactory, Func<string, T> from, Func<T, string> to)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.defaultValueFactory = defaultValueFactory;
        this.from = from;
        this.to = to;
    }

    public ClassUsingCustomDbProperty(IServiceProvider serviceProvider, string key, T defaultValue, Func<string, T> from, Func<T, string> to)
        : this(serviceProvider, key, () => defaultValue, from, to)
    {
    }

    [field: MaybeNull]
    public override T Value
    {
        get
        {
            if (field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = GetValue(appDbContext, key);
                    field = string.IsNullOrEmpty(value)
                        ? defaultValueFactory()
                        : from(value);
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

    protected override void SetValue(T value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, to(value)));
        }
    }
}