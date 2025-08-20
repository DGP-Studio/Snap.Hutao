// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using System.Runtime.CompilerServices;

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

    [field: AllowNull]
    public override string Value
    {
        get => GetOption(ref field, key, defaultValueFactory);
        set => SetOption(ref field, key, value);
    }

    private string GetOption(ref string? storage, string key, Func<string> defaultValueFactory)
    {
        if (storage is not null)
        {
            return storage;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            storage = GetValue(appDbContext, key) ?? defaultValueFactory();
        }

        return storage;
    }

    private bool SetOption(ref string? storage, string key, string value, [CallerMemberName] string? propertyName = null)
    {
        if (!SetProperty(ref storage, value, propertyName))
        {
            return false;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, value));
        }

        return true;
    }
}