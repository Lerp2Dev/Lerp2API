using UnityEngine;
using Lerp2API;
using Lerp2API.Hepers.Serializer_Helpers;

/// <summary>
/// Class StoreMesh.
/// </summary>
public class StoreMesh : MonoBehaviour
{
    /// <summary>
    /// The vertices
    /// </summary>
    [HideInInspector]
    public Vector3[] vertices;

    /// <summary>
    /// The normals
    /// </summary>
    [HideInInspector]
    public Vector3[] normals;

    /// <summary>
    /// The uv
    /// </summary>
    [HideInInspector]
    public Vector2[] uv;

    /// <summary>
    /// The uv1
    /// </summary>
    [HideInInspector]
    public Vector2[] uv1;

    /// <summary>
    /// The uv2
    /// </summary>
    [HideInInspector]
    public Vector2[] uv2;

    /// <summary>
    /// The colors
    /// </summary>
    [HideInInspector]
    public Color[] colors;

    /// <summary>
    /// The triangles
    /// </summary>
    [HideInInspector]
    public int[][] triangles;

    /// <summary>
    /// The tangents
    /// </summary>
    [HideInInspector]
    public Vector4[] tangents;

    /// <summary>
    /// The sub mesh count
    /// </summary>
    [HideInInspector]
    public int subMeshCount;

    private MeshFilter filter;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Awake()
    {
        filter = GetComponent<MeshFilter>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (filter == null && skinnedMeshRenderer == null)
            Destroy(this);
    }

    private void OnSerializing()
    { //This is not called on Editor
        var mesh = filter != null ? filter.mesh : skinnedMeshRenderer.sharedMesh;
        vertices = mesh.vertices;
        normals = mesh.normals;
        uv = mesh.uv;
        uv1 = mesh.uv2;
        uv2 = mesh.uv2;
        colors = mesh.colors;
        triangles = new int[subMeshCount = mesh.subMeshCount][];
        for (var i = 0; i < mesh.subMeshCount; ++i)
            triangles[i] = mesh.GetTriangles(i);
        tangents = mesh.tangents;
    }

    private void OnDeserialized()
    { //So, we need to mod a little bit
        bool av = true;
        try
        { //First, try to know if the data come from Editor
            Vector3[] v = vertices;
            if (v.Length == 0)
                av = false;
        }
        catch
        { //If it's from Editor nothing wouldn't be stored, so we have to take action in it...
            av = false; // vv As we saved it before in GameObject_DirectConvert, we can restore it safely!
        }
        Mesh mesh = new Mesh();
        if (av)
        {
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.uv2 = uv1;
            mesh.uv2 = uv2;
            mesh.colors = colors;
            mesh.tangents = tangents;
            mesh.subMeshCount = subMeshCount;
            for (var i = 0; i < subMeshCount; ++i)
                mesh.SetTriangles(triangles[i], i);
            mesh.RecalculateBounds();
            if (filter != null)
                filter.mesh = mesh;
            else if (skinnedMeshRenderer != null)
                skinnedMeshRenderer.sharedMesh = mesh;
        }
        else
        { //If it's from Editor nothing wouldn't be stored, so we have to take action in it...
            byte[] b = SerializerHelpers.LoadAsset("Mesh/" + LevelLoader.Current.Last.name + ".asset");
            if (b == null)
            {
                Debug.LogErrorFormat("{0} mesh couldn't be loaded.", LevelLoader.Current.Last.name);
                return;
            }
            mesh = MeshSerializer.ReadMesh(b); //As we saved it before in GameObject_DirectConvert, we can restore it safely!
            if (mesh == null)
            {
                Debug.LogFormat("{0} couldn't be rebuilt.", LevelLoader.Current.Last.name);
                return;
            }
        }
        if (filter != null)
            filter.mesh = mesh;
        else if (skinnedMeshRenderer != null)
            skinnedMeshRenderer.sharedMesh = mesh;
    }
}