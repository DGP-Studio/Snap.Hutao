// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Linq.Expressions;

namespace Snap.Hutao.Core.ExpressionService;

/// <summary>
/// 枚举帮助类
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 判断枚举是否有对应的Flag
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="enum">待检查的枚举</param>
    /// <param name="value">值</param>
    /// <returns>是否有对应的Flag</returns>
    public static bool HasOption<T>(this T @enum, T value)
        where T : struct, Enum
    {
        return ExpressionCache<T>.Entry(@enum, value);
    }

    private static class ExpressionCache<T>
    {
        public static readonly Func<T, T, bool> Entry = Get();

        private static Func<T, T, bool> Get()
        {
            ParameterExpression paramSource = Expression.Parameter(typeof(T));
            ParameterExpression paramValue = Expression.Parameter(typeof(T));

            BinaryExpression logicalAnd = Expression.AndAssign(paramSource, paramValue);
            BinaryExpression equal = Expression.Equal(logicalAnd, paramValue);

            // 生成一个源类型入，目标类型出的 lamdba
            return Expression.Lambda<Func<T, T, bool>>(equal, paramSource, paramValue).Compile();
        }
    }
}