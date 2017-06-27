#if !NO_UNITY

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        /// <summary>
        /// The register GUI style state direct converter
        /// </summary>
        public static Internal.DirectConverters.GUIStyleState_DirectConverter Register_GUIStyleState_DirectConverter;
    }
}

namespace FullSerializer.Internal.DirectConverters
{
    /// <summary>
    /// Class GUIStyleState_DirectConverter.
    /// </summary>
    /// <seealso cref="FullSerializer.fsDirectConverter{UnityEngine.GUIStyleState}" />
    public class GUIStyleState_DirectConverter : fsDirectConverter<GUIStyleState>
    {
        /// <summary>
        /// Does the serialize.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serialized">The serialized.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoSerialize(GUIStyleState model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "background", model.background);
            result += SerializeMember(serialized, null, "textColor", model.textColor);

            return result;
        }

        /// <summary>
        /// Does the deserialize.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="model">The model.</param>
        /// <returns>fsResult.</returns>
        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyleState model)
        {
            var result = fsResult.Success;

            var t0 = model.background;
            result += DeserializeMember(data, null, "background", out t0);
            model.background = t0;

            var t2 = model.textColor;
            result += DeserializeMember(data, null, "textColor", out t2);
            model.textColor = t2;

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
            return new GUIStyleState();
        }
    }
}

#endif