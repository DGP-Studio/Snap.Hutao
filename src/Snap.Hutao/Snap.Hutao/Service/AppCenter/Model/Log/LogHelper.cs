// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public static class LogHelper
{
    public static T Initialize<T>(this T log, Guid sid, Device device)
        where T : Log
    {
        log.Session = sid;
        log.Device = device;

        return log;
    }

    public static AppCenterException Create(Exception exception)
    {
        AppCenterException current = new()
        {
            Type = exception.GetType().ToString(),
            Message = exception.Message,
            StackTrace = exception.StackTrace,
        };

        if (exception is AggregateException aggregateException)
        {
            if (aggregateException.InnerExceptions.Count != 0)
            {
                current.InnerExceptions = new();
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    current.InnerExceptions.Add(Create(innerException));
                }
            }
        }

        if (exception.InnerException != null)
        {
            current.InnerExceptions ??= new();
            current.InnerExceptions.Add(Create(exception.InnerException));
        }

        StackTrace stackTrace = new(exception, true);
        StackFrame[] frames = stackTrace.GetFrames();

        if (frames.Length > 0 && frames[0].HasNativeImage())
        {
        }

        return current;
    }
}