#if !NO_UNITY

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FullSerializer
{
    partial class fsConverterRegistrar
    {
        public static Internal.DirectConverters.GameObject_DirectConverter Register_GameObject_DirectConverter;
    }
}

namespace FullSerializer.Internal.DirectConverters
{
    public class GameObject_DirectConverter : fsDirectConverter<GameObject>
    { //With help of: https://dzone.com/articles/convert-object-byte-array-and, FullSerializer, SerializeHelper
        protected override fsResult DoSerialize(GameObject model, Dictionary<string, fsData> serialized)
        {
            var result = fsResult.Success;

            if (model.hideFlags != HideFlags.None)
                model.hideFlags = HideFlags.None;

            if (!model.GetComponent<MeshRenderer>().enabled)
                model.GetComponent<MeshRenderer>().enabled = true;

            GameObject go = Object.Instantiate(model);
            go.name = model.name;

            //We should check for Editing Mode (because it's only required if we are serializing the desried GameObject in Editor),
            //but, why? It's more safe and more slow :) (This remember me to PHP haha)
            GameObject foundGo = null;
            Mesh mesh = PluginsHelper.FindMesh(go, ref foundGo);
            if (mesh != null)
                MeshSerializer.WriteMesh(mesh, true).SaveAsAsset(mesh, foundGo.name);

            byte[] bytes = go.SaveObjectTree();
            string path = bytes.SaveAsAsset(go, go.name);

            if (Application.isPlaying)
                Object.Destroy(go);
            else
                Object.DestroyImmediate(go);

            result += SerializeMember(serialized, null, "path", path);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GameObject model)
        {
            var result = fsResult.Success;

            string path;
            result += DeserializeMember(data, null, "path", out path);

            byte[] bytes = SerializerHelpers.LoadAsset(path);

            model = null;

            if (bytes == null)
                return result;

            bytes.LoadObjectTree();

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return null; //This creates empty gameobjects in scene, I have to fork code and edit it for me.
        }
    }
}

#endif