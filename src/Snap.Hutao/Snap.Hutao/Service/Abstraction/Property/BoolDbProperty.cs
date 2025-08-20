// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class BoolDbProperty : DbProperty<bool>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly Func<bool> defaultValueFactory;
    private bool? field;

    public BoolDbProperty(IServiceProvider serviceProvider, string key, Func<bool> defaultValueFactory)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.defaultValueFactory = defaultValueFactory;
    }

    public BoolDbProperty(IServiceProvider serviceProvider, string key, bool defaultValue)
        :this(serviceProvider, key, () => defaultValue)
    {
    }

    public override bool Value
    {
        get => GetOption(ref @field, key, defaultValueFactory);
        set => SetOption(ref @field, key, value);
    }

    private bool GetOption(ref bool? storage, string key, Func<bool> defaultValueFactory)
    {
        if (storage is not null)
        {
            return storage.Value;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = GetValue(appDbContext, key);
            storage = value is null ? defaultValueFactory() : bool.Parse(value);
        }

        return storage.Value;
    }

    private bool SetOption(ref bool? storage, string key, bool value, [CallerMemberName] string? propertyName = null)
    {
        if (!SetProperty(ref storage, value, propertyName))
        {
            return false;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, value ? "true" : "false"));
        }

        return true;
    }
}