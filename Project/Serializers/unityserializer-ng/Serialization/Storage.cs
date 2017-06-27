using Serialization;
using System;

/// <summary>
/// Class Storage.
/// </summary>
public static class Storage
{
    //Serialize an object to a compressed format in a BASE64 string
    /// <summary>
    /// Serializes to string.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>System.String.</returns>
    public static string SerializeToString(this object obj)
    {
        return Convert.ToBase64String(UnitySerializer.Serialize(obj));
    }

    //Typed deserialization
    //public static T Deserialize<T>(this string data) where T : class {
    //    return Deserialize(data) as T;
    //}

    //Deserialize a compressed object from a string
    /// <summary>
    /// Deserializes the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>System.Object.</returns>
    public static object Deserialize(string data)
    {
        return UnitySerializer.Deserialize(Convert.FromBase64String(data));
    }
}