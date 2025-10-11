// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Data.Xml.Dom;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

/// <summary>
/// Base notification content interface to retrieve notification Xml as a string.
/// </summary>
internal interface INotificationContent
{
    /// <summary>
    /// Retrieves the notification Xml content as a string.
    /// </summary>
    /// <returns>The notification Xml content as a string.</returns>
    string GetContent();


    /// <summary>
    /// Retrieves the notification Xml content as a WinRT Xml document.
    /// </summary>
    /// <returns>The notification Xml content as a WinRT Xml document.</returns>
    XmlDocument GetXml();
}