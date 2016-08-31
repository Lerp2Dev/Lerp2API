#if !NO_UNITY

using System;
using System.Collections.Generic;
using UnityEngine;
using Lerp2API;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        public static Internal.DirectConverters.Texture2D_DirectConverter Register_Texture2D_DirectConverter;
    }
}

namespace FullSerializer.Internal.DirectConverters
{
    public class Texture2D_DirectConverter : fsDirectConverter<Texture2D>
    {
        protected override fsResult DoSerialize(Texture2D model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "width", model.width);

            result += SerializeMember(serialized, null, "height", model.height);

            result += SerializeMember(serialized, null, "format", model.format);

            byte[] bytes = model.GetRawTextureData();
            string path = bytes.SaveAsAsset(model);
            result += SerializeMember(serialized, null, "path", path);

            float mipmapBias = model.mipMapBias;
            TextureWrapMode wrapMode = model.wrapMode;
            FilterMode filterMode = model.filterMode;
            int anisoLevel = model.anisoLevel;

            result += SerializeMember(serialized, null, "mipmapBias", mipmapBias);

            result += SerializeMember(serialized, null, "wrapMode", wrapMode);

            result += SerializeMember(serialized, null, "filterMode", filterMode);

            result += SerializeMember(serialized, null, "anisoLevel", anisoLevel);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Texture2D model)
        {
            var result = fsResult.Success;

            int width;
            result += DeserializeMember(data, null, "width", out width);

            int height;
            result += DeserializeMember(data, null, "height", out height);

            TextureFormat format;
            result += DeserializeMember(data, null, "format", out format);

            string path;
            result += DeserializeMember(data, null, "path", out path);

            float mipmapBias;
            result += DeserializeMember(data, null, "mipmapBias", out mipmapBias);

            TextureWrapMode wrapMode;
            result += DeserializeMember(data, null, "wrapMode", out wrapMode);

            FilterMode filterMode;
            result += DeserializeMember(data, null, "filterMode", out filterMode);

            int anisoLevel;
            result += DeserializeMember(data, null, "anisoLevel", out anisoLevel);

            model = new Texture2D(width, height, format, mipmapBias >= 0, filterMode == FilterMode.Point);
            model.mipMapBias = mipmapBias;
            model.wrapMode = wrapMode;
            model.filterMode = filterMode;
            model.anisoLevel = anisoLevel;
            model.LoadRawTextureData(SerializerHelpers.LoadAsset(path));
            model.Apply();

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new Texture2D(1, 1);
        }
    }
}

#endif