using System.Collections.Generic;
using UnityEngine;

namespace Lerp2API.Hepers.Serializer_Helpers
{
    /// <summary>
    /// Class PluginsHelper.
    /// </summary>
    public partial class PluginsHelper
    {
        /// <summary>
        /// The object list
        /// </summary>
        public static Dictionary<string, GameObject> objList = new Dictionary<string, GameObject>();

        /// <summary>
        /// Adds the specified go.
        /// </summary>
        /// <param name="go">The go.</param>
        public static void Add(GameObject go)
        {
            GameObject root = go.getRoot();
            if (!objList.ContainsKey(root.name))
                objList.Add(root.name, root);
        }

        /// <summary>
        /// Finds the mesh filter.
        /// </summary>
        /// <param name="go">The go.</param>
        /// <param name="foundGo">The found go.</param>
        /// <returns>MeshFilter.</returns>
        public static MeshFilter FindMeshFilter(GameObject go, ref GameObject foundGo)
        {
            if (go == null)
            {
                foundGo = null;
                return null;
            }
            if (go.GetComponent<MeshFilter>() != null)
            {
                foundGo = go;
                return go.GetComponent<MeshFilter>();
            }
            foreach (Transform child in go.transform)
            {
                MeshFilter mf = FindMeshFilter(child.gameObject, ref foundGo);
                if (mf != null)
                {
                    foundGo = child.gameObject;
                    return mf;
                }
                else
                    continue;
            }
            return null;
        }

        /// <summary>
        /// Finds the skinned mesh renderer.
        /// </summary>
        /// <param name="go">The go.</param>
        /// <param name="foundGo">The found go.</param>
        /// <returns>SkinnedMeshRenderer.</returns>
        public static SkinnedMeshRenderer FindSkinnedMeshRenderer(GameObject go, ref GameObject foundGo)
        {
            if (go == null)
            {
                foundGo = null;
                return null;
            }
            if (go.GetComponent<SkinnedMeshRenderer>() != null)
            {
                foundGo = go;
                return go.GetComponent<SkinnedMeshRenderer>();
            }
            foreach (Transform child in go.transform)
            {
                SkinnedMeshRenderer mr = FindSkinnedMeshRenderer(child.gameObject, ref foundGo);
                if (mr != null)
                {
                    foundGo = child.gameObject;
                    return mr;
                }
                else
                    continue;
            }
            return null;
        }

        /// <summary>
        /// Finds the mesh.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="foundGo">The found go.</param>
        /// <returns>Mesh.</returns>
        public static Mesh FindMesh(GameObject obj, ref GameObject foundGo)
        {
            MeshFilter mf = FindMeshFilter(obj, ref foundGo);
            SkinnedMeshRenderer smr = FindSkinnedMeshRenderer(obj, ref foundGo);
            if (mf != null)
            {
                if (Application.isPlaying)
                    return mf.mesh;
                else
                    return mf.sharedMesh;
            }
            else if (smr != null)
                return smr.sharedMesh;
            else
                return null;
        }
    }
}