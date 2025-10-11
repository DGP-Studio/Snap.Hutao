// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("audio")]
internal sealed class ElementToastAudio
{
    internal const bool DefaultLoop = false;
    internal const bool DefaultSilent = false;

    /// <summary>
    /// The media file to play in place of the default sound. This can either be a ms-winsoundevent value, or a custom ms-appx:/// or ms-appdata:/// file, or null for the default sound.
    /// </summary>
    [NotificationXmlAttribute("src")]
    public Uri? Src { get; set; }

    [NotificationXmlAttribute("loop", DefaultLoop)]
    public bool Loop { get; set; }

    /// <summary>
    /// True to mute the sound; false to allow the toast notification sound to play.
    /// </summary>
    [NotificationXmlAttribute("silent", DefaultSilent)]
    public bool Silent { get; set; }
}