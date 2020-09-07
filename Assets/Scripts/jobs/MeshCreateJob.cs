using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct MeshCreateJob : IJob
{

    public NativeArray<BlockData> blockData;
    public NativeList<ChunkMeshVertexData> verts;
    public int vertCount;
    public NativeList<int> tris;
    public int triCount;

    public int sizex;
    public int sizey;
    public int sizez;

    public int x;
    public int y;
    public int z;

    public NativeArray<int> counts;



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

    public void Execute()
    {


        



        int vertIndex = 0;
        int triIndex = 0;

        x = -1;
        for (int i = 0; i < blockData.Length; i++)
        {

            x = x + 1;
            // convert the straight array to xyz coords  // maybe not necessary
            if (x == sizex)
            {
                x = 0;
                y = y+1;

                if (y == sizey)
                {
                    y = 0;
                    z = z + 1;
                }
            }



            BlockData block = blockData[i];


            if (! block.isVisible)
            {
                continue;
            }

            for (int face = 0; face < 6; face++)
            {


                if (! isNeighborSolid(blockData, i, face))
                {
                    // add the face to the mesh

                    //ChunkMeshVertexData vertsout0 = verts[vertIndex];
                    //ChunkMeshVertexData vertsout1 = verts[vertIndex +1];
                    //ChunkMeshVertexData vertsout2 = verts[vertIndex +2];
                    //ChunkMeshVertexData vertsout3 = verts[vertIndex +3];

                    ChunkMeshVertexData vertsout0 = new ChunkMeshVertexData();
                    ChunkMeshVertexData vertsout1 = new ChunkMeshVertexData();
                    ChunkMeshVertexData vertsout2 = new ChunkMeshVertexData();
                    ChunkMeshVertexData vertsout3 = new ChunkMeshVertexData();


                    vertsout0.pos = _vertices[_faceVertices[face, 0]] + new Vector3(x, y, z);
                    vertsout1.pos = _vertices[_faceVertices[face, 1]] + new Vector3(x, y, z);
                    vertsout2.pos = _vertices[_faceVertices[face, 2]] + new Vector3(x, y, z);
                    vertsout3.pos = _vertices[_faceVertices[face, 3]] + new Vector3(x, y, z);

                    vertsout0.normal = _normals[face];
                    vertsout1.normal = _normals[face];
                    vertsout2.normal = _normals[face];
                    vertsout3.normal = _normals[face];


                    vertsout0.uv = new Vector2(0, 0);
                    vertsout1.uv = new Vector2(0, 1);
                    vertsout2.uv = new Vector2(1, 1);
                    vertsout3.uv = new Vector2(1, 0);

                    //verts[vertIndex] = vertsout0;
                    //verts[vertIndex+1] = vertsout1;
                    //verts[vertIndex+2] = vertsout2;
                    //verts[vertIndex+3] = vertsout3;

                    verts.Add(vertsout0);
                    verts.Add(vertsout1);
                    verts.Add(vertsout2);
                    verts.Add(vertsout3);



                    //tris[triIndex] = _triangleVertices[face, 0] + vertIndex;
                    //tris[triIndex + 1] = _triangleVertices[face, 1] + vertIndex;
                    //tris[triIndex + 2] = _triangleVertices[face, 2] + vertIndex;
                    //tris[triIndex + 3] = _triangleVertices[face, 3] + vertIndex;
                    //tris[triIndex + 4] = _triangleVertices[face, 4] + vertIndex;
                    //tris[triIndex + 5] = _triangleVertices[face, 5] + vertIndex;


                    tris.Add(_triangleVertices[face, 0] + vertIndex);
                    tris.Add(_triangleVertices[face, 1] + vertIndex);
                    tris.Add(_triangleVertices[face, 2] + vertIndex);
                    tris.Add(_triangleVertices[face, 3] + vertIndex);
                    tris.Add(_triangleVertices[face, 4] + vertIndex);
                    tris.Add(_triangleVertices[face, 5] + vertIndex);


                    vertIndex += 4;
                    triIndex += 6;

                }


            }

        }

        counts[0] = vertIndex;
        counts[1] = triIndex;

    }


    private Boolean isNeighborSolid(NativeArray<BlockData> blockData, int index, int face)
    {

        int neighborIndex = -1;


        switch(face)
        {
            case 0: 
                if (y < sizey -1)
                {
                    neighborIndex = index + sizex;
                }
                break;
            case 1:
                if (y > 0)
                {
                    neighborIndex = index - sizex;
                }
                break;
            case 2:
                if (z < sizez - 1)
                {
                    neighborIndex = index + sizex * sizey;
                }
                break;
            case 3:
                if (z > 0)
                {
                    neighborIndex = index - sizex * sizey;
                }
                break;
            case 4:
                if (x < sizex - 1)
                {
                    neighborIndex = index + 1;
                }
                break;
            case 5:
                if (x > 0)
                {
                    neighborIndex = index - 1;
                }
                break;
        }


        if (neighborIndex < 0)
        {
            return false;
        }

        return blockData[neighborIndex].isSolid;

    }



}

