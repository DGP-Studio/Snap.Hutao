// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Image.Implementation;

internal sealed class ImageExFailedEventArgs : EventArgs
{
    public ImageExFailedEventArgs(Exception errorException)
    {
        ErrorMessage = ErrorException?.Message;
        ErrorException = errorException;
    }

    public Exception? ErrorException { get; private set; }

    public string? ErrorMessage { get; private set; }
}
