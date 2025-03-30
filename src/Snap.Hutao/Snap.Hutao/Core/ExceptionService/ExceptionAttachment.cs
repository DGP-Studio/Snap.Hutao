// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ExceptionService;

internal static class ExceptionAttachment
{
    private static readonly ConditionalWeakTable<Exception, SentryAttachment> Attachments = [];

    public static void SetAttachment(Exception exception, SentryAttachment? attachment)
    {
        if (attachment is null)
        {
            return;
        }

        Attachments.Add(exception, attachment);
    }

    public static bool TryGetAttachment(Exception exception, [NotNullWhen(true)] out SentryAttachment? attachment)
    {
        return Attachments.TryGetValue(exception, out attachment);
    }
}