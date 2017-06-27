using Serialization;
using System;
using System.IO;
using System.Xml.Serialization;

/// <summary>
/// Class XmlSupport.
/// </summary>
public static class XmlSupport
{
    /// <summary>
    /// Deserializes the XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xml">The XML.</param>
    /// <returns>T.</returns>
    public static T DeserializeXml<T>(this string xml) where T : class
    {
        var s = new XmlSerializer(typeof(T));
        using (var m = new MemoryStream(UnitySerializer.TextEncoding.GetBytes(xml)))
        {
            return (T)s.Deserialize(m);
        }
    }

    /// <summary>
    /// Deserializes the XML.
    /// </summary>
    /// <param name="xml">The XML.</param>
    /// <param name="tp">The tp.</param>
    /// <returns>System.Object.</returns>
    public static object DeserializeXml(this string xml, Type tp)
    {
        var s = new XmlSerializer(tp);
        using (var m = new MemoryStream(UnitySerializer.TextEncoding.GetBytes(xml)))
        {
            return s.Deserialize(m);
        }
    }

    /// <summary>
    /// Serializes the XML.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>System.String.</returns>
    public static string SerializeXml(this object item)
    {
        var s = new XmlSerializer(item.GetType());
        using (var m = new MemoryStream())
        {
            s.Serialize(m, item);
            m.Flush();
            return UnitySerializer.TextEncoding.GetString(m.GetBuffer());
        }
    }
}