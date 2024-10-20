// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal static class ValueResultExtension
{
    public static bool TryGetValue<TValue>(this in ValueResult<bool, TValue> valueResult, [NotNullWhen(true)][MaybeNullWhen(false)] out TValue value)
    {
        value = valueResult.Value;
        return valueResult.IsOk;
    }
}