#if !NO_UNITY

using System;
using UnityEngine;
using UnityEngine.Events;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        /// <summary>
        /// The register unity event converter
        /// </summary>
        public static Internal.Converters.UnityEvent_Converter Register_UnityEvent_Converter;
    }
}

namespace FullSerializer.Internal.Converters
{
    // The standard FS reflection converter has started causing Unity to crash when processing
    // UnityEvent. We can send the serialization through JsonUtility which appears to work correctly
    // instead.
    //
    // We have to support legacy serialization formats so importing works as expected.
    /// <summary>
    /// Class UnityEvent_Converter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsConverter" />
    public class UnityEvent_Converter : fsConverter
    {
        /// <summary>
        /// Can this converter serialize and deserialize the given object type?
        /// </summary>
        /// <param name="type">The given object type.</param>
        /// <returns>True if the converter can serialize it, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return typeof(UnityEvent).Resolve().IsAssignableFrom(type) && type.IsGenericType == false;
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
        /// Deserialize data into the object instance.
        /// </summary>
        /// <param name="data">Serialization data to deserialize from.</param>
        /// <param name="instance">The object instance to deserialize into.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>True if serialization was successful, false otherwise.</returns>
        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            Type objectType = (Type)instance;

            fsResult result = fsResult.Success;
            instance = JsonUtility.FromJson(fsJsonPrinter.CompressedJson(data), objectType);
            return result;
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
            fsResult result = fsResult.Success;
            serialized = fsJsonParser.Parse(JsonUtility.ToJson(instance));
            return result;
        }
    }
}

#endif