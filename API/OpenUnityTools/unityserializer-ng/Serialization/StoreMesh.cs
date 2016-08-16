using UnityEngine;
using System.IO;

public class StoreMesh : MonoBehaviour
{

    [HideInInspector]
    public Vector3[] vertices;
    [HideInInspector]
    public Vector3[] normals;
    [HideInInspector]
    public Vector2[] uv;
    [HideInInspector]
    public Vector2[] uv1;
    [HideInInspector]
    public Vector2[] uv2;
    [HideInInspector]
    public Color[] colors;
    [HideInInspector]
    public int[][] triangles;
    [HideInInspector]
    public Vector4[] tangents;
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
        if (av)
        {
            var mesh = new Mesh();
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
            byte[] b = SerializerHelpers.LoadAsset(Path.Combine(Application.streamingAssetsPath, "Mesh/" + LevelLoader.Current.Last.name + ".asset"), false);
            if (b == null)
                return;
            Mesh mesh = MeshSerializer.ReadMesh(b); //As we saved it before in GameObject_DirectConvert, we can restore it safely!
            if (mesh == null)
                return;
            if (filter != null)
                filter.mesh = mesh;
            else if (skinnedMeshRenderer != null)
                skinnedMeshRenderer.sharedMesh = mesh;
        }
    }
}