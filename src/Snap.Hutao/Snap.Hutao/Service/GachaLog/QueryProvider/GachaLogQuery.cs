// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

internal readonly struct GachaLogQuery
{
    public readonly string Message;
    public readonly string Query;
    public readonly bool IsOversea;

    public GachaLogQuery(string query)
    {
        Query = query;
        IsOversea = query.Contains("region=os_", StringComparison.OrdinalIgnoreCase);
        Message = string.Empty;
    }

    private GachaLogQuery(string query, string message)
    {
        Query = query;
        Message = message;
    }

    public bool IsInvalid { get => string.IsNullOrEmpty(Message); }

    public static GachaLogQuery Invalid(string message)
    {
        return new(string.Empty, message);
    }
}