using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;

/// <summary>
/// Defines the <see cref="MeshCreateJob" />.
/// </summary>
[BurstCompile]
public struct MultiMeshCreateJob : IJobParallelFor
{


    // Inputs
    public NativeArray<ulong> blockDataPtrs;
    public NativeArray<int> bockDataCounts;

    [NativeDisableParallelForRestriction]
    public NativeArray<int3> chunkSize;




    //Output
    public NativeArray<ulong> vertsPtrs;
    public NativeArray<int> vertCount;
    public NativeArray<ulong> trisPtrs;
    public NativeArray<int> triIntCounts;



    // TODO:  May want to clean some of this up, but it does run relatively fast.
    /// <summary>
    /// The Execute.
    /// </summary>
    public unsafe void Execute(int index)
    {

        // Convert the pointers to data to try it out
        // Block Data
        void* ptr = (void*)blockDataPtrs[index];
        NativeArray<ChunkBlockData> blockData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ChunkBlockData>(ptr, bockDataCounts[index], Allocator.None);
        AtomicSafetyHandle blockDataAtomicHandle = AtomicSafetyHandle.Create();
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref blockData, blockDataAtomicHandle);


        NativeArray<ChunkMeshVertexData> _verts = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ChunkMeshVertexData>((void*)vertsPtrs[index], bockDataCounts[index] * 24, Allocator.None);
        AtomicSafetyHandle vertsAtomicHandle = AtomicSafetyHandle.Create();
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref _verts, vertsAtomicHandle);


        NativeArray<int> _tris = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)trisPtrs[index], bockDataCounts[index] * 36, Allocator.None);
        AtomicSafetyHandle trisAtomicHandle = AtomicSafetyHandle.Create();
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref _tris, trisAtomicHandle);



        int3 size = (int3)chunkSize[0];



        //var verts = new NativeList<ChunkMeshVertexData>(0, Allocator.Temp);
        //var tris = new NativeList<int>(0, Allocator.Temp);





        //verts.Capacity = 60000;
        //tris.Capacity = 60000;
        int vertIndex = 0;
        int triIndex = 0;


        int x = 0;
        int y = 0;
        int z = 0;


        x = -1;
        for (int i = 0; i < blockData.Length; i++)
        {




            x = x + 1;
            // convert the straight array to xyz coords  // maybe not necessary
            if (x == size.x)
            {
                x = 0;
                y = y + 1;

                if (y == size.y)
                {
                    y = 0;
                    z = z + 1;
                }
            }

            ChunkBlockData block = blockData[i];


            if (!block.isVisible)
            {

                continue;
            }




            for (int face = 0; face < 6; face++)
            {


                Boolean neighborSolid = false;

                int neighborIndex = -1;


                    if (face == 0 && y < size.y - 1)
                    {
                        neighborIndex = i + size.x;
                    }
                    else if (face == 1 && y > 0)
                    {
                        neighborIndex = i - size.x;
                    }
                    else if (face == 2 && z < size.z - 1)
                    {
                        neighborIndex = i + size.x * size.y;
                    }
                    else if (face == 3 && z > 0)
                    {
                        neighborIndex = i - size.x * size.y;
                    }
                    else if (face == 4 && x < size.x - 1)
                    {
                        neighborIndex = i + 1;
                    }
                    else if (face == 5 && x > 0)
                    {
                        neighborIndex = i - 1;
                    }





                if (neighborIndex >= 0)
                {
                    neighborSolid = blockData[neighborIndex].isSolid;
                }


                if (!neighborSolid)
                {
                    // add the face to the mesh
                    ChunkMeshVertexData vertsout0 = new ChunkMeshVertexData();
                    ChunkMeshVertexData vertsout1 = new ChunkMeshVertexData();
                    ChunkMeshVertexData vertsout2 = new ChunkMeshVertexData();
                    ChunkMeshVertexData vertsout3 = new ChunkMeshVertexData();

                    //the static vertex data needs to be expanded so the burst compiler can work properly


                    vertsout0.pos = _vertices[_faceVertices2[face * 4 + 0]] + new Vector3(x, y, z);
                    vertsout1.pos = _vertices[_faceVertices2[face * 4 + 1]] + new Vector3(x, y, z);
                    vertsout2.pos = _vertices[_faceVertices2[face * 4 + 2]] + new Vector3(x, y, z);
                    vertsout3.pos = _vertices[_faceVertices2[face * 4 + 3]] + new Vector3(x, y, z);



                    vertsout0.normal = _normals[face];
                    vertsout1.normal = _normals[face];
                    vertsout2.normal = _normals[face];
                    vertsout3.normal = _normals[face];


                    vertsout0.uv = new Vector2(0, 0);
                    vertsout1.uv = new Vector2(0, 1);
                    vertsout2.uv = new Vector2(1, 1);
                    vertsout3.uv = new Vector2(1, 0);

                    _verts[vertIndex] = vertsout0;
                    _verts[vertIndex + 1] = vertsout1;
                    _verts[vertIndex + 2] = vertsout2;
                    _verts[vertIndex + 3] = vertsout3;


                    _tris[triIndex] = _triangleVertices2[face * 6 + 0] + vertIndex;
                    _tris[triIndex + 1] = _triangleVertices2[face * 6 + 1] + vertIndex;
                    _tris[triIndex + 2] = _triangleVertices2[face * 6 + 2] + vertIndex;
                    _tris[triIndex + 3] = _triangleVertices2[face * 6 + 3] + vertIndex;
                    _tris[triIndex + 4] = _triangleVertices2[face * 6 + 4] + vertIndex;
                    _tris[triIndex + 5] = _triangleVertices2[face * 6 + 5] + vertIndex;



                    vertIndex += 4;
                    triIndex += 6;


                }
            }


        }

        vertCount[index] = vertIndex;
        triIntCounts[index] = triIndex;

        AtomicSafetyHandle.Release(blockDataAtomicHandle);
        AtomicSafetyHandle.Release(vertsAtomicHandle);
        AtomicSafetyHandle.Release(trisAtomicHandle);

    }




    readonly internal static Vector2[] _uvs = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
        };

    readonly internal static Vector3[] _normals = new Vector3[]
    {
        new Vector3(0,1,0), // Top
        new Vector3(0,-1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),

    };

    readonly internal static Vector3[] _vertices = new Vector3[]
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


    readonly internal static int[] _faceVertices2 = new int[]
    {
            2, 6, 7, 3,
            4, 0, 1, 5,
            5, 7, 6, 4,
            0, 2, 3, 1,
            1, 3, 7, 5,
            4, 6, 2, 0
    };



    // Need to use a single d array in a job
    readonly internal static int[] _triangleVertices2 = new int[]
    {
            0, 1, 3, 3, 1, 2 ,
            0, 1, 3, 3, 1, 2 ,
            0, 1, 3, 3, 1, 2 ,
            0, 1, 3, 3, 1, 2 ,
            0, 1, 3, 3, 1, 2 ,
            0, 1, 3, 3, 1, 2
    };


    // TODO: make this come from the block rather than hard coding
    internal static int[] materialIndex = new int[]
        { 12, 13, 14,15,8, 9 };







}
