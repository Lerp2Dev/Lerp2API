using UnityEngine;
using Serialization;

[SpecialistProvider]
public class TimeAsFloat : ISpecialist {
    #region ISpecialist implementation
    public object Serialize(object value) {
        return (float)value - Time.time;
    }

    public object Deserialize(object value) {
        return Time.time + (float)value;
    }
    #endregion
}