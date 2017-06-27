using System;

namespace FullSerializer.Internal
{
    /// <summary>
    /// Class fsPrimitiveConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsConverter" />
    public class fsPrimitiveConverter : fsConverter
    {
        /// <summary>
        /// Can this converter serialize and deserialize the given object type?
        /// </summary>
        /// <param name="type">The given object type.</param>
        /// <returns>True if the converter can serialize it, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return
                type.Resolve().IsPrimitive ||
                type == typeof(string) ||
                type == typeof(decimal);
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

        private static bool UseBool(Type type)
        {
            return type == typeof(bool);
        }

        private static bool UseInt64(Type type)
        {
            return type == typeof(sbyte) || type == typeof(byte) ||
                   type == typeof(Int16) || type == typeof(UInt16) ||
                   type == typeof(Int32) || type == typeof(UInt32) ||
                   type == typeof(Int64) || type == typeof(UInt64);
        }

        private static bool UseDouble(Type type)
        {
            return type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }

        private static bool UseString(Type type)
        {
            return type == typeof(string) ||
                   type == typeof(char);
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
            var instanceType = instance.GetType();

            if (Serializer.Config.Serialize64BitIntegerAsString && (instanceType == typeof(Int64) || instanceType == typeof(UInt64)))
            {
                serialized = new fsData((string)Convert.ChangeType(instance, typeof(string)));
                return fsResult.Success;
            }

            if (UseBool(instanceType))
            {
                serialized = new fsData((bool)instance);
                return fsResult.Success;
            }

            if (UseInt64(instanceType))
            {
                serialized = new fsData((Int64)Convert.ChangeType(instance, typeof(Int64)));
                return fsResult.Success;
            }

            if (UseDouble(instanceType))
            {
                // Casting from float to double introduces floating point jitter, ie, 0.1 becomes 0.100000001490116.
                // Casting to decimal as an intermediate step removes the jitter. Not sure why.
                if (instance.GetType() == typeof(float) &&
                    // Decimal can't store float.MinValue/float.MaxValue/float.PositiveInfinity/float.NegativeInfinity/float.NaN - an exception gets thrown in that scenario.
                    (float)instance != float.MinValue &&
                    (float)instance != float.MaxValue &&
                    !float.IsInfinity((float)instance) &&
                    !float.IsNaN((float)instance)
                    )
                {
                    serialized = new fsData((double)(decimal)(float)instance);
                    return fsResult.Success;
                }

                serialized = new fsData((double)Convert.ChangeType(instance, typeof(double)));
                return fsResult.Success;
            }

            if (UseString(instanceType))
            {
                serialized = new fsData((string)Convert.ChangeType(instance, typeof(string)));
                return fsResult.Success;
            }

            serialized = null;
            return fsResult.Fail("Unhandled primitive type " + instance.GetType());
        }

        /// <summary>
        /// Tries the deserialize.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="storageType">Type of the storage.</param>
        /// <returns>fsResult.</returns>
        public override fsResult TryDeserialize(fsData storage, ref object instance, Type storageType)
        {
            var result = fsResult.Success;

            if (UseBool(storageType))
            {
                if ((result += CheckType(storage, fsDataType.Boolean)).Succeeded)
                {
                    instance = storage.AsBool;
                }
                return result;
            }

            if (UseDouble(storageType) || UseInt64(storageType))
            {
                if (storage.IsDouble)
                {
                    instance = Convert.ChangeType(storage.AsDouble, storageType);
                }
                else if (storage.IsInt64)
                {
                    instance = Convert.ChangeType(storage.AsInt64, storageType);
                }
                else if (Serializer.Config.Serialize64BitIntegerAsString && storage.IsString &&
                    (storageType == typeof(Int64) || storageType == typeof(UInt64)))
                {
                    instance = Convert.ChangeType(storage.AsString, storageType);
                }
                else {
                    return fsResult.Fail(GetType().Name + " expected number but got " + storage.Type + " in " + storage);
                }
                return fsResult.Success;
            }

            if (UseString(storageType))
            {
                if ((result += CheckType(storage, fsDataType.String)).Succeeded)
                {
                    instance = storage.AsString;
                }
                return result;
            }

            return fsResult.Fail(GetType().Name + ": Bad data; expected bool, number, string, but got " + storage);
        }
    }
}