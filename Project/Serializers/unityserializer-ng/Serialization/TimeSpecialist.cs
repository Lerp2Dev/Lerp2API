using Serialization;
using UnityEngine;

/// <summary>
/// Class TimeAsFloat.
/// </summary>
[SpecialistProvider]
public class TimeAsFloat : ISpecialist
{
    #region ISpecialist implementation

    /// <summary>
    /// Serializes the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public object Serialize(object value)
    {
        return (float)value - Time.time;
    }

    /// <summary>
    /// Deserializes the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Object.</returns>
    public object Deserialize(object value)
    {
        return Time.time + (float)value;
    }

    #endregion ISpecialist implementation
}