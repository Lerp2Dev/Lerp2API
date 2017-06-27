using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
    /// <summary>
    /// Class fsKeyValuePairConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsConverter" />
    public class fsKeyValuePairConverter : fsConverter
    {
        /// <summary>
        /// Can this converter serialize and deserialize the given object type?
        /// </summary>
        /// <param name="type">The given object type.</param>
        /// <returns>True if the converter can serialize it, false otherwise.</returns>
        public override bool CanProcess(Type type)
        {
            return
                type.Resolve().IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
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
        /// Deserialize data into the object instance.
        /// </summary>
        /// <param name="data">Serialization data to deserialize from.</param>
        /// <param name="instance">The object instance to deserialize into.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>True if serialization was successful, false otherwise.</returns>
        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            var result = fsResult.Success;

            fsData keyData, valueData;
            if ((result += CheckKey(data, "Key", out keyData)).Failed) return result;
            if ((result += CheckKey(data, "Value", out valueData)).Failed) return result;

            var genericArguments = storageType.GetGenericArguments();
            Type keyType = genericArguments[0], valueType = genericArguments[1];

            object keyObject = null, valueObject = null;
            result.AddMessages(Serializer.TryDeserialize(keyData, keyType, ref keyObject));
            result.AddMessages(Serializer.TryDeserialize(valueData, valueType, ref valueObject));

            instance = Activator.CreateInstance(storageType, keyObject, valueObject);
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
            PropertyInfo keyProperty = storageType.GetDeclaredProperty("Key");
            PropertyInfo valueProperty = storageType.GetDeclaredProperty("Value");

            object keyObject = keyProperty.GetValue(instance, null);
            object valueObject = valueProperty.GetValue(instance, null);

            var genericArguments = storageType.GetGenericArguments();
            Type keyType = genericArguments[0], valueType = genericArguments[1];

            var result = fsResult.Success;

            fsData keyData, valueData;
            result.AddMessages(Serializer.TrySerialize(keyType, keyObject, out keyData));
            result.AddMessages(Serializer.TrySerialize(valueType, valueObject, out valueData));

            serialized = fsData.CreateDictionary();
            if (keyData != null) serialized.AsDictionary["Key"] = keyData;
            if (valueData != null) serialized.AsDictionary["Value"] = valueData;

            return result;
        }
    }
}