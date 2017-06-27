using System;
using System.Collections.Generic;

namespace FullSerializer
{
    /// <summary>
    /// The direct converter is similar to a regular converter, except that it targets specifically only one type.
    /// This means that it can be used without performance impact when discovering converters. It is strongly
    /// recommended that you derive from fsDirectConverter{TModel}.
    /// </summary>
    /// <remarks>Due to the way that direct converters operate, inheritance is *not* supported. Direct converters
    /// will only be used with the exact ModelType object.</remarks>
    public abstract class fsDirectConverter : fsBaseConverter
    {
        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public abstract Type ModelType { get; }
    }

    /// <summary>
    /// Class fsDirectConverter.
    /// </summary>
    /// <typeparam name="TModel">The type of the t model.</typeparam>
    /// <seealso cref="FullSerializer.fsDirectConverter" />
    public abstract class fsDirectConverter<TModel> : fsDirectConverter
    {
        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public override Type ModelType { get { return typeof(TModel); } }

        /// <summary>
        /// Serialize the actual object into the given data storage.
        /// </summary>
        /// <param name="instance">The object instance to serialize. This will never be null.</param>
        /// <param name="serialized">The serialized state.</param>
        /// <param name="storageType">The field/property type that is storing this instance.</param>
        /// <returns>If serialization was successful.</returns>
        public sealed override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            var serializedDictionary = new Dictionary<string, fsData>();
            var result = DoSerialize((TModel)instance, serializedDictionary);
            serialized = new fsData(serializedDictionary);
            return result;
        }

        /// <summary>
        /// Deserialize data into the object instance.
        /// </summary>
        /// <param name="data">Serialization data to deserialize from.</param>
        /// <param name="instance">The object instance to deserialize into.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>True if serialization was successful, false otherwise.</returns>
        public sealed override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            var result = fsResult.Success;
            if ((result += CheckType(data, fsDataType.Object)).Failed) return result;

            var obj = (TModel)instance;
            result += DoDeserialize(data.AsDictionary, ref obj);
            instance = obj;
            return result;
        }

        /// <summary>
        /// Does the serialize.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serialized">The serialized.</param>
        /// <returns>fsResult.</returns>
        protected abstract fsResult DoSerialize(TModel model, Dictionary<string, fsData> serialized);

        /// <summary>
        /// Does the deserialize.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="model">The model.</param>
        /// <returns>fsResult.</returns>
        protected abstract fsResult DoDeserialize(Dictionary<string, fsData> data, ref TModel model);
    }
}