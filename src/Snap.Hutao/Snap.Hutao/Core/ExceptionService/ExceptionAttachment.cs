// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;

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

    public static void SetAttachment(Exception exception, string fileName, byte[] data)
    {
        ByteAttachmentContent attachmentContent = new(data);
        SentryAttachment attachment = new(AttachmentType.Default, attachmentContent, fileName, MediaTypeNames.Text.Plain);
        SetAttachment(exception, attachment);
    }

    public static void SetAttachment(Exception exception, string fileName, string data)
    {
        SetAttachment(exception, fileName, Encoding.UTF8.GetBytes(data));
    }

    public static bool TryGetAttachment(Exception exception, [NotNullWhen(true)] out SentryAttachment? attachment)
    {
        return Attachments.TryGetValue(exception, out attachment);
    }
}