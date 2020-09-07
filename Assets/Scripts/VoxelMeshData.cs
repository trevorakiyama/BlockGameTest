using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMeshData
{

    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uvs;
    public List<Vector3> normals;


    public VoxelMeshData(List<Vector3> verts, List<int> tris, List<Vector2> uvs, List<Vector3> norms)
    {
        this.vertices = verts;
        this.triangles = tris;
        this.uvs = uvs;
        this.normals = norms;

    }


}
