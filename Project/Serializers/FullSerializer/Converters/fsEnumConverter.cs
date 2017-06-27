using System;
using System.Collections.Generic;
using System.Text;

namespace FullSerializer.Internal
{
    /// <summary>
    /// Serializes and deserializes enums by their current name.
    /// </summary>
    public class fsEnumConverter : fsConverter
    {
        /// <summary>
        /// Can this converter serialize and deserialize the given object type?
        /// </summary>
        /// <param name="type">The given object type.</param>
        /// <returns>True if the converter can serialize it, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return type.Resolve().IsEnum;
        }

        /// <summary>
        /// If true, then the serializer will support cyclic references with the given converted
        /// type.
        /// </summary>
        /// <param name="storageType">The field/property type that is currently storing the object
        /// that is being serialized.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        /// <summary>
        /// If true, then the serializer will include inheritance data for the given converter.
        /// </summary>
        /// <param name="storageType">The field/property type that is currently storing the object
        /// that is being serialized.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        /// <summary>
        /// Construct an object instance that will be passed to TryDeserialize. This should **not**
        /// deserialize the object.
        /// </summary>
        /// <param name="data">The data the object was serialized with.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>An object instance</returns>
        public override object CreateInstance(fsData data, Type storageType)
        {
            // In .NET compact, Enum.ToObject(Type, Object) is defined but the overloads like
            // Enum.ToObject(Type, int) are not -- so we get around this by boxing the value.
            return Enum.ToObject(storageType, (object)0);
        }

        /// <summary>
        /// Serialize the actual object into the given data storage.
        /// </summary>
        /// <param name="instance">The object instance to serialize. This will never be null.</param>
        /// <param name="serialized">The serialized state.</param>
        /// <param name="storageType">The field/property type that is storing this instance.</param>
        /// <returns>If serialization was successful.</returns>
        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            if (Serializer.Config.SerializeEnumsAsInteger)
            {
                serialized = new fsData(Convert.ToInt64(instance));
            }
            else if (fsPortableReflection.GetAttribute<FlagsAttribute>(storageType) != null)
            {
                long instanceValue = Convert.ToInt64(instance);
                var result = new StringBuilder();

                bool first = true;
                foreach (var value in Enum.GetValues(storageType))
                {
                    long integralValue = Convert.ToInt64(value);
                    bool isSet = (instanceValue & integralValue) != 0;

                    if (isSet)
                    {
                        if (first == false) result.Append(",");
                        first = false;
                        result.Append(value.ToString());
                    }
                }

                serialized = new fsData(result.ToString());
            }
            else {
                serialized = new fsData(Enum.GetName(storageType, instance));
            }
            return fsResult.Success;
        }

        /// <summary>
        /// Deserialize data into the object instance.
        /// </summary>
        /// <param name="data">Serialization data to deserialize from.</param>
        /// <param name="instance">The object instance to deserialize into.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>True if serialization was successful, false otherwise.</returns>
        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            if (data.IsString)
            {
                string[] enumValues = data.AsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                long instanceValue = 0;
                for (int i = 0; i < enumValues.Length; ++i)
                {
                    string enumValue = enumValues[i];

                    // Verify that the enum name exists; Enum.TryParse is only available in .NET 4.0
                    // and above :(.
                    if (ArrayContains(Enum.GetNames(storageType), enumValue) == false)
                    {
                        return fsResult.Fail("Cannot find enum name " + enumValue + " on type " + storageType);
                    }

                    long flagValue = (long)Convert.ChangeType(Enum.Parse(storageType, enumValue), typeof(long));
                    instanceValue |= flagValue;
                }

                instance = Enum.ToObject(storageType, (object)instanceValue);
                return fsResult.Success;
            }
            else if (data.IsInt64)
            {
                int enumValue = (int)data.AsInt64;

                // In .NET compact, Enum.ToObject(Type, Object) is defined but the overloads like
                // Enum.ToObject(Type, int) are not -- so we get around this by boxing the value.
                instance = Enum.ToObject(storageType, (object)enumValue);

                return fsResult.Success;
            }

            return fsResult.Fail("EnumConverter encountered an unknown JSON data type");
        }

        /// <summary>
        /// Returns true if the given value is contained within the specified array.
        /// </summary>
        private static bool ArrayContains<T>(T[] values, T value)
        {
            // note: We don't use LINQ because this function will *not* allocate
            for (int i = 0; i < values.Length; ++i)
            {
                if (EqualityComparer<T>.Default.Equals(values[i], value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}