// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Linq.Expressions;

namespace Snap.Hutao.Core.Convert;

/// <summary>
/// Class to cast to type <see cref="TTo"/>
/// </summary>
/// <typeparam name="TTo">Target type</typeparam>
public static class CastTo<TTo>
{
    /// <summary>
    /// Casts <see cref="s"/> to <see cref="TTo"/>.
    /// This does not cause boxing for value types.
    /// Useful in generic methods.
    /// </summary>
    /// <typeparam name="TFrom">Source type to cast from. Usually a generic type.</typeparam>
    /// <param name="from">from value</param>
    /// <returns>target value</returns>
    public static TTo From<TFrom>(TFrom from)
    {
        return Cache<TFrom>.Caster(from);
    }

    private static class Cache<TCachedFrom>
    {
        public static readonly Func<TCachedFrom, TTo> Caster = Get();

        private static Func<TCachedFrom, TTo> Get()
        {
            // 参数表达式，表示 传入源类型
            ParameterExpression param = Expression.Parameter(typeof(TCachedFrom));

            // 一元转换 调用 相关类的显式或隐式转换运算符
            UnaryExpression convert = Expression.ConvertChecked(param, typeof(TTo));

            // 生成一个源类型入，目标类型出的 lamdba
            return Expression.Lambda<Func<TCachedFrom, TTo>>(convert, param).Compile();
        }
    }
}