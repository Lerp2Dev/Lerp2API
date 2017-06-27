using System;
using System.Globalization;

namespace FullSerializer.Internal
{
    /// <summary>
    /// Supports serialization for DateTime, DateTimeOffset, and TimeSpan.
    /// </summary>
    public class fsDateConverter : fsConverter
    {
        // The format strings that we use when serializing DateTime and DateTimeOffset types.
        private const string DefaultDateTimeFormatString = @"o";

        private const string DateTimeOffsetFormatString = @"o";

        private string DateTimeFormatString
        {
            get
            {
                return Serializer.Config.CustomDateTimeFormatString ?? DefaultDateTimeFormatString;
            }
        }

        /// <summary>
        /// Can this converter serialize and deserialize the given object type?
        /// </summary>
        /// <param name="type">The given object type.</param>
        /// <returns>True if the converter can serialize it, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan);
        }

        /// <summary>
        /// Serialize the actual object into the given data storage.
        /// </summary>
        /// <param name="instance">The object instance to serialize. This will never be null.</param>
        /// <param name="serialized">The serialized state.</param>
        /// <param name="storageType">The field/property type that is storing this instance.</param>
        /// <returns>If serialization was successful.</returns>
        /// <exception cref="InvalidOperationException">FullSerializer Internal Error -- Unexpected serialization type</exception>
        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            if (instance is DateTime)
            {
                var dateTime = (DateTime)instance;
                serialized = new fsData(dateTime.ToString(DateTimeFormatString));
                return fsResult.Success;
            }

            if (instance is DateTimeOffset)
            {
                var dateTimeOffset = (DateTimeOffset)instance;
                serialized = new fsData(dateTimeOffset.ToString(DateTimeOffsetFormatString));
                return fsResult.Success;
            }

            if (instance is TimeSpan)
            {
                var timeSpan = (TimeSpan)instance;
                serialized = new fsData(timeSpan.ToString());
                return fsResult.Success;
            }

            throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected serialization type");
        }

        /// <summary>
        /// Deserialize data into the object instance.
        /// </summary>
        /// <param name="data">Serialization data to deserialize from.</param>
        /// <param name="instance">The object instance to deserialize into.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>True if serialization was successful, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">FullSerializer Internal Error -- Unexpected deserialization type</exception>
        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            if (data.IsString == false)
            {
                return fsResult.Fail("Date deserialization requires a string, not " + data.Type);
            }

            if (storageType == typeof(DateTime))
            {
                DateTime result;
                if (DateTime.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out result))
                {
                    instance = result;
                    return fsResult.Success;
                }

                // DateTime.TryParse can fail for some valid DateTime instances. Try to use Convert.ToDateTime.
                if (fsGlobalConfig.AllowInternalExceptions)
                {
                    try
                    {
                        instance = Convert.ToDateTime(data.AsString);
                        return fsResult.Success;
                    }
                    catch (Exception e)
                    {
                        return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTime; got exception " + e);
                    }
                }

                return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTime");
            }

            if (storageType == typeof(DateTimeOffset))
            {
                DateTimeOffset result;
                if (DateTimeOffset.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out result))
                {
                    instance = result;
                    return fsResult.Success;
                }

                return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTimeOffset");
            }

            if (storageType == typeof(TimeSpan))
            {
                TimeSpan result;
                if (TimeSpan.TryParse(data.AsString, out result))
                {
                    instance = result;
                    return fsResult.Success;
                }

                return fsResult.Fail("Unable to parse " + data.AsString + " into a TimeSpan");
            }

            throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected deserialization type");
        }
    }
}