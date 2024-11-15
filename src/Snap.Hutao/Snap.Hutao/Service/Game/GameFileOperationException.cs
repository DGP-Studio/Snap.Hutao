// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

internal sealed class GameFileOperationException : Exception
{
    public GameFileOperationException(string message, Exception? innerException)
        : base(SH.FormatServiceGameFileOperationExceptionMessage(message), innerException)
    {
    }
}