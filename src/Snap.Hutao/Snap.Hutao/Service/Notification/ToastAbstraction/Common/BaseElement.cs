// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text;
using System.Xml;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

internal abstract class BaseElement
{
    /// <summary>
    /// Retrieves the notification XML content as a string.
    /// </summary>
    /// <returns>The notification XML content as a string.</returns>
    public string GetContent()
    {
        using (MemoryStream stream = new())
        {
            XmlWriterSettings settings = new()
            {
                Encoding = Encoding.UTF8, // Use UTF-8 encoding to save space (it defaults to UTF-16 which is 2x the size)
                Indent = true,
                NewLineOnAttributes = true,
            };
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                XmlWriterHelper.Write(writer, this);
            }

            stream.Position = 0;

            using (StreamReader reader = new(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Retrieves the notification XML content as a WinRT XML document.
    /// </summary>
    /// <returns>The notification XML content as a WinRT XML document.</returns>
    public Windows.Data.Xml.Dom.XmlDocument GetXml()
    {
        Windows.Data.Xml.Dom.XmlDocument xml = new();
        xml.LoadXml(GetContent());
        return xml;
    }
}