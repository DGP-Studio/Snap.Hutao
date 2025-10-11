// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Specify audio to be played when the toast notification is received.
/// </summary>
internal sealed class ToastAudio
{
    /// <summary>
    /// Initializes a new instance of toast audio, which specifies what audio to play when the toast notification is received.
    /// </summary>
    public ToastAudio()
    {
    }

    /// <summary>
    /// The media file to play in place of the default sound.
    /// </summary>
    public Uri? Src { get; set; }

    /// <summary>
    /// Set to true if the sound should repeat as long as the toast is shown; false to play only once (default).
    /// </summary>
    public bool Loop { get; set; } = ElementToastAudio.DefaultLoop;

    /// <summary>
    /// True to mute the sound; false to allow the toast notification sound to play (default).
    /// </summary>
    public bool Silent { get; set; } = ElementToastAudio.DefaultSilent;

    internal ElementToastAudio ConvertToElement()
    {
        return new()
        {
            Src = Src,
            Loop = Loop,
            Silent = Silent
        };
    }
}