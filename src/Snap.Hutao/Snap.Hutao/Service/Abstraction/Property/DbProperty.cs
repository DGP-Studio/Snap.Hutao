// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity.Database;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Abstraction.Property;

internal abstract class DbProperty<T> : ObservableObject, IProperty<T>
{
    public abstract T Value { get; set; }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected static string? GetValue(AppDbContext appDbContext, string key)
    {
        // This method is separated to avoid implicit capture of the key
        return appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
    }
}

[SuppressMessage("", "SA1204")]
internal static partial class DbProperty
{
    public static DbProperty<TSource> Link<TSource, TTarget>(this DbProperty<TSource> source, DbProperty<TTarget> target, [RequireStaticDelegate] Action<TSource, DbProperty<TTarget>> callback)
    {
        return new LinkedDbProperty<TSource, TTarget>(source, target, callback);
    }

    public static DbProperty<bool> AlsoSetFalseWhenFalse(this DbProperty<bool> source, DbProperty<bool> target)
    {
        return Link(source, target, static (value, target) =>
        {
            if (!value)
            {
                target.Value = false;
            }
        });
    }

    private sealed partial class LinkedDbProperty<TSource, TTarget> : DbProperty<TSource>
    {
        private readonly DbProperty<TSource> source;
        private readonly DbProperty<TTarget> target;
        private readonly Action<TSource, DbProperty<TTarget>> callback;

        public LinkedDbProperty(DbProperty<TSource> source, DbProperty<TTarget> target, Action<TSource, DbProperty<TTarget>> callback)
        {
            this.source = source;
            this.target = target;
            this.callback = callback;

            this.source.PropertyChanged += OnSourceValueChanged;
        }

        public override TSource Value
        {
            get => source.Value;
            set => source.Value = value;
        }

        private void OnSourceValueChanged(object? sender, PropertyChangedEventArgs e)
        {
            callback(source.Value, target);
        }
    }
}