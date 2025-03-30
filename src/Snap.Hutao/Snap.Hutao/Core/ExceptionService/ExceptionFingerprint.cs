// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ExceptionService;

internal static class ExceptionFingerprint
{
    private static readonly ConditionalWeakTable<Exception, string> Fingerprints = [];

    public static void SetFingerprint(Exception exception, string? fingerprint)
    {
        if (fingerprint is null)
        {
            return;
        }

        Fingerprints.Add(exception, fingerprint);
    }

    public static bool TryGetFingerprint(Exception exception, [NotNullWhen(true)] out string? fingerprint)
    {
        return Fingerprints.TryGetValue(exception, out fingerprint);
    }
}