// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;
using System.Reflection;
using System.Xml;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

internal static class XmlWriterHelper
{
    public static void Write(XmlWriter writer, object element)
    {
        // If it isn't an element attribute, don't write anything
        if (GetElementAttribute(element.GetType()) is not { } elAttr)
        {
            return;
        }

        writer.WriteStartElement(elAttr.Name);

        IEnumerable<PropertyInfo> properties = GetProperties(element.GetType());

        List<object> elements = [];
        object? content = null;

        // Write the attributes first
        foreach (PropertyInfo p in properties)
        {
            IEnumerable<Attribute> attributes = GetCustomAttributes(p);

            NotificationXmlAttributeAttribute attr = attributes.OfType<NotificationXmlAttributeAttribute>().FirstOrDefault();

            object? propertyValue = GetPropertyValue(p, element);

            // If it's an attribute
            if (attr is not null)
            {
                object? defaultValue = attr.DefaultValue;

                // If the value is not the default value (and it's not null) we'll write it
                if (!Equals(propertyValue, defaultValue) && propertyValue is not null)
                {
                    writer.WriteAttributeString(attr.Name, PropertyValueToString(propertyValue));
                }
            }

            // If it's a content attribute
            else if (attributes.OfType<NotificationXmlContentAttribute>().Any())
            {
                content = propertyValue;
            }

            // Otherwise it's an element or collection of elements
            else
            {
                if (propertyValue != null)
                {
                    elements.Add(propertyValue);
                }
            }
        }

        // Then write children
        foreach (object el in elements)
        {
            // If it's a collection of children
            if (el is IEnumerable enumerable)
            {
                foreach (object child in enumerable)
                {
                    Write(writer, child);
                }

                continue;
            }

            // Otherwise just write the single element
            Write(writer, el);
        }

        // Then write any content if there is content
        if (content != null)
        {
            string? contentString = content.ToString();
            if (!string.IsNullOrWhiteSpace(contentString))
            {
                writer.WriteString(contentString);
            }
        }

        writer.WriteEndElement();
    }

    private static object? GetPropertyValue(PropertyInfo propertyInfo, object obj)
    {
        return propertyInfo.GetValue(obj, null);
    }

    private static string? PropertyValueToString(object propertyValue)
    {
        Type type = propertyValue.GetType();

        if (IsEnum(type))
        {
            EnumStringAttribute? enumStringAttr = GetEnumStringAttribute(propertyValue as Enum);

            if (enumStringAttr != null)
                return enumStringAttr.String;
        }

        else if (propertyValue is bool value)
        {
            return value ? "true" : "false";
        }

        return propertyValue.ToString();
    }

    private static EnumStringAttribute? GetEnumStringAttribute(Enum? enumValue)
    {
        MemberInfo[]? memberInfo = enumValue?.GetType().GetMember(enumValue.ToString());

        if (memberInfo != null && memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(EnumStringAttribute), false);

            if (attrs.Length > 0)
                return attrs[0] as EnumStringAttribute;
        }

        return null;
    }

    private static bool IsEnum(Type type)
    {
        return type.IsEnum;
    }

    private static IEnumerable<PropertyInfo> GetProperties(Type type)
    {
        return type.GetProperties();
    }

    private static NotificationXmlElementAttribute? GetElementAttribute(Type type)
    {
        return GetCustomAttributes(type).OfType<NotificationXmlElementAttribute>().FirstOrDefault();
    }

    private static IEnumerable<Attribute> GetCustomAttributes(Type type)
    {
        return type.GetCustomAttributes(true).OfType<Attribute>();
    }

    private static IEnumerable<Attribute> GetCustomAttributes(PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttributes(true).OfType<Attribute>();
    }
}