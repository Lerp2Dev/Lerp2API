using UnitySerializerNG.FilePreferences;

/// <summary>
/// Class FilePrefs.
/// </summary>
public static class FilePrefs
{
    private static DataContainer<string> stringData = new DataContainer<string>("str");
    private static DataContainer<float> floatData = new DataContainer<float>("fpn");
    private static DataContainer<int> intData = new DataContainer<int>("int");

    /// <summary>
    /// Deletes all.
    /// </summary>
    public static void DeleteAll()
    {
        stringData.Clear();
        floatData.Clear();
        intData.Clear();
    }

    /// <summary>
    /// Deletes the key.
    /// </summary>
    /// <param name="key">The key.</param>
    public static void DeleteKey(string key)
    {
        stringData.Remove(key);
        floatData.Remove(key);
        intData.Remove(key);
    }

    /// <summary>
    /// Gets the float.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.Single.</returns>
    public static float GetFloat(string key)
    {
        return floatData.Get(key);
    }

    /// <summary>
    /// Gets the int.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.Int32.</returns>
    public static int GetInt(string key)
    {
        return intData.Get(key);
    }

    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    public static string GetString(string key)
    {
        return stringData.Get(key);
    }

    /// <summary>
    /// Determines whether the specified key has key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if the specified key has key; otherwise, <c>false</c>.</returns>
    public static bool HasKey(string key)
    {
        return stringData.Find(key) || floatData.Find(key) || intData.Find(key);
    }

    /// <summary>
    /// Saves this instance.
    /// </summary>
    public static void Save()
    {
        stringData.Save();
        floatData.Save();
        intData.Save();
    }

    /// <summary>
    /// Sets the float.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetFloat(string key, float value)
    {
        floatData.Set(key, value);
    }

    /// <summary>
    /// Sets the int.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetInt(string key, int value)
    {
        intData.Set(key, value);
    }

    /// <summary>
    /// Sets the string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void SetString(string key, string value)
    {
        stringData.Set(key, value);
    }
}