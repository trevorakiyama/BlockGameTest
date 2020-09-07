using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public class Voxel
{

    enum FACE
    {
        TOP,
        BOTTOM,
        NORTH,
        SOUTH,
        EAST,
        WEST
    }

    static Vector2[] _uvs = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
        };

    static Vector3[] _normals = new Vector3[]
    {
        new Vector3(0,1,0), // Top
        new Vector3(0,-1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),

    };

    static Vector3[] _vertices = new Vector3[]
    {
        new Vector3(0,0,0), // 0
        new Vector3(1,0,0), // 1
        new Vector3(0,1,0), // 2
        new Vector3(1,1,0), // 3
        new Vector3(0,0,1), // 4
        new Vector3(1,0,1), // 5
        new Vector3(0,1,1), // 6
        new Vector3(1,1,1)  // 7
    };

    static int[,] _faceVertices = new int[,]
 {
        { 2, 6, 7, 3},
        { 4, 0, 1, 5},
        { 5, 7, 6, 4},
        { 0, 2, 3, 1},
        { 1, 3, 7, 5},
        { 4, 6, 2, 0}
 };

    static int[,] _triangleVertices = new int[,]
    {
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 }
    };



    // TODO: make this come from the block rather than hard coding
    static int[] materialIndex = new int[]
        { 12, 13, 14,15,8, 9 };



    // default block voxel renderer
    public static VoxelMeshData getVoxelMeshData(Boolean[] renderFaces, Vector3Int offset)
    {

        int textureIndex = 0;

        List<VoxelFace> faces = new List<VoxelFace>();

        for (int i = 0; i < 6; i++)
        {
            if (renderFaces[i] == false)
            {
                continue;
            }

            VoxelFace face = getBlockVoxelFace(offset, i, textureIndex);
            faces.Add(face);
        }

        return GetVoxelMeshFromFaces(faces);
    }

    internal static int addVoxelMeshData(bool[] renderFaces, Vector3Int posInChunk, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals, int vertexCount)
    {

        int addedVertices = 0;

        for (int i = 0; i < 6; i++ )
        {

            if (renderFaces[i])
            {
                vertices.Add(_vertices[_faceVertices[i, 0]] + posInChunk);
                vertices.Add(_vertices[_faceVertices[i, 1]] + posInChunk);
                vertices.Add(_vertices[_faceVertices[i, 2]] + posInChunk);
                vertices.Add(_vertices[_faceVertices[i, 3]] + posInChunk);



                (Vector2 v1, Vector2 v2) =  TextureManager.GetUVCoords(materialIndex[i]);


                uvs.Add(v1);
                uvs.Add(new Vector2(v1.x, v2.y));
                uvs.Add(v2);
                uvs.Add(new Vector2(v2.x, v1.y));

                normals.Add(_normals[i]);
                normals.Add(_normals[i]);
                normals.Add(_normals[i]);
                normals.Add(_normals[i]);

                triangles.Add(_triangleVertices[i, 0] + vertexCount + addedVertices);
                triangles.Add(_triangleVertices[i, 1] + vertexCount + addedVertices);
                triangles.Add(_triangleVertices[i, 2] + vertexCount + addedVertices);
                triangles.Add(_triangleVertices[i, 3] + vertexCount + addedVertices);
                triangles.Add(_triangleVertices[i, 4] + vertexCount + addedVertices);
                triangles.Add(_triangleVertices[i, 5] + vertexCount + addedVertices);

                addedVertices += 4;
            }
        }

        return addedVertices;
    }







    private static VoxelMeshData GetVoxelMeshFromFaces(List<VoxelFace> faces)
    {

        if (faces.Count < 1)
        {
            return null;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        int faceCount = 0;
        foreach (VoxelFace face in faces)
        {

            // copy the Vertex information
            for (int i = 0; i < 4; i++)
            {
                vertices.Add(face.vertices[i]);
                uvs.Add(face.uvs[i]);
                normals.Add(face.normals[i]);
            }

            for (int i = 0; i < 6; i++)
            {
                triangles.Add(face.triangles[i] + faceCount * 4);
            }

            faceCount++;
        }

        VoxelMeshData voxelMeshData = new VoxelMeshData(vertices, triangles, uvs, normals);

        return voxelMeshData;
    }

    public static VoxelFace getBlockVoxelFace(Vector3 offset, int faceId, int textureIndex)
    {

        VoxelFace face = new VoxelFace();

        face.vertices[0] = _vertices[_faceVertices[faceId, 0]] + offset;
        face.vertices[1] = _vertices[_faceVertices[faceId, 1]] + offset;
        face.vertices[2] = _vertices[_faceVertices[faceId, 2]] + offset;
        face.vertices[3] = _vertices[_faceVertices[faceId, 3]] + offset;

        // TODO Make  a texture manager Component to get textures, for now Hard code to a single texture

        face.uvs[0] = _uvs[0];
        face.uvs[1] = _uvs[1];
        face.uvs[2] = _uvs[2];
        face.uvs[3] = _uvs[3];

        Vector3 normal = new Vector3(0, 0, -1);
        face.normals[0] = _normals[faceId];
        face.normals[1] = _normals[faceId];
        face.normals[2] = _normals[faceId];
        face.normals[3] = _normals[faceId];

        face.triangles[0] = _triangleVertices[faceId, 0];
        face.triangles[1] = _triangleVertices[faceId, 1];
        face.triangles[2] = _triangleVertices[faceId, 2];
        face.triangles[3] = _triangleVertices[faceId, 3];
        face.triangles[4] = _triangleVertices[faceId, 4];
        face.triangles[5] = _triangleVertices[faceId, 5];

        return face;
    }

}
