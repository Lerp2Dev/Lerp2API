#if !NO_UNITY

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        /// <summary>
        /// The register layer mask direct converter
        /// </summary>
        public static Internal.DirectConverters.LayerMask_DirectConverter Register_LayerMask_DirectConverter;
    }
}

namespace FullSerializer.Internal.DirectConverters
{
    /// <summary>
    /// Class LayerMask_DirectConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsDirectConverter{UnityEngine.LayerMask}" />
    public class LayerMask_DirectConverter : fsDirectConverter<LayerMask>
    {
        /// <summary>
        /// Does the serialize.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serialized">The serialized.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoSerialize(LayerMask model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "value", model.value);

            return result;
        }

        /// <summary>
        /// Does the deserialize.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="model">The model.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref LayerMask model)
        {
            var result = fsResult.Success;

            var t0 = model.value;
            result += DeserializeMember(data, null, "value", out t0);
            model.value = t0;

            return result;
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
            return new LayerMask();
        }
    }
}

#endif