// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity.Database;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.Service.Abstraction.Property;

internal sealed partial class SelectedOneBasedIndexDbProperty : DbProperty<NameValue<int>?>
{
    private readonly IServiceProvider serviceProvider;
    private readonly string key;
    private readonly ImmutableArray<NameValue<int>> array;

    public SelectedOneBasedIndexDbProperty(IServiceProvider serviceProvider, string key, ImmutableArray<NameValue<int>> array)
    {
        this.serviceProvider = serviceProvider;
        this.key = key;
        this.array = array;
    }

    public override NameValue<int>? Value
    {
        get
        {
            if (field is null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    string? value = GetValue(appDbContext, key);
                    field = string.IsNullOrEmpty(value) ? array.FirstOrDefault() : RestrictIndex(array, value);
                }
            }

            return field;
        }

        set
        {
            if (value is null)
            {
                return;
            }

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

    protected override void SetValue(NameValue<int>? value)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, $"{value?.Value}"));
        }
    }

    private static NameValue<int>? RestrictIndex(ImmutableArray<NameValue<int>> array, string value)
    {
        return array.IsDefaultOrEmpty
            ? default
            : array[Math.Clamp(int.Parse(value, CultureInfo.InvariantCulture) - 1, 0, array.Length - 1)];
    }
}