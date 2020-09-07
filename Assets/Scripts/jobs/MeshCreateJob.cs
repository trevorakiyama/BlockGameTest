using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;

/// <summary>
/// Defines the <see cref="MeshCreateJob" />.
/// </summary>
[BurstCompile]
public struct MeshCreateJob : IJob
{
    public NativeArray<BlockData> blockData;

    public NativeList<ChunkMeshVertexData> verts;
    public NativeList<int> tris;

    public int vertCount;
    public int triCount;

    public int sizex;

    public int sizey;

    public int sizez;

    public int x;

    public int y;

    public int z;

    public int mode;


    public NativeArray<int> counts;

    internal static Vector2[] _uvs = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
        };

    internal static Vector3[] _normals = new Vector3[]
    {
        new Vector3(0,1,0), // Top
        new Vector3(0,-1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),

    };

    internal static Vector3[] _vertices = new Vector3[]
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

    internal static int[,] _faceVertices = new int[,]
 {
        { 2, 6, 7, 3},
        { 4, 0, 1, 5},
        { 5, 7, 6, 4},
        { 0, 2, 3, 1},
        { 1, 3, 7, 5},
        { 4, 6, 2, 0}
 };

    internal static int[,] _triangleVertices = new int[,]
    {
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 },
        { 0, 1, 3, 3, 1, 2 }
    };

    // TODO: make this come from the block rather than hard coding
    internal static int[] materialIndex = new int[]
        { 12, 13, 14,15,8, 9 };



    



    /// <summary>
    /// The Execute.
    /// </summary>
    public void Execute()
    {

        //ProfilerMarker markerFull = new ProfilerMarker("Full");


        //ProfilerMarker markerInit = new ProfilerMarker("Init");

        //ProfilerMarker markerPreMain = new ProfilerMarker("PreMain");
        //ProfilerMarker markerInMain = new ProfilerMarker("mloop");


        //ProfilerMarker marker1 = new ProfilerMarker("1");
        //ProfilerMarker marker2 = new ProfilerMarker("2");
        //ProfilerMarker marker3 = new ProfilerMarker("3");
        //ProfilerMarker marker4 = new ProfilerMarker("4");
        //ProfilerMarker marker4n = new ProfilerMarker("4.n");
        //ProfilerMarker marker41 = new ProfilerMarker("4.1");


        //markerFull.Begin();
        //markerInit.Begin();

        verts.Capacity = 60000;
        tris.Capacity = 60000;

        int vertIndex = 0;
        int triIndex = 0;


        // Convert to managed array for faster read access
        BlockData[] newBlockData = blockData.ToArray();



        //markerInit.End();


        //int face = 0;


        //markerPreMain.Begin();

        x = -1;
        for (int i = 0; i < blockData.Length; i++)
        {

            //markerInMain.Begin();

            //marker1.Begin();
            if (true || mode == 0)
            {
                
                x = x + 1;
                // convert the straight array to xyz coords  // maybe not necessary
                if (x == sizex)
                {
                    x = 0;
                    y = y + 1;

                    if (y == sizey)
                    {
                        y = 0;
                        z = z + 1;
                    }
                }
                

            }
            //marker1.End();


            //marker2.Begin();
            BlockData block = newBlockData[i];
            

            if (!block.isVisible)
            {
                //marker2.End();
                //markerInMain.End();
                continue;
            }
            //marker2.End();

            //marker3.Begin();

            for (int face = 0; face < 6; face++)
            {

                //marker4.Begin();

                //marker4n.Begin();


                // 

                Boolean neighborSolid = false;

                int neighborIndex = -1;


                switch (face)
                {
                    case 0:
                        if (y < sizey - 1)
                        {
                            neighborIndex = i + sizex;
                        }
                        break;
                    case 1:
                        if (y > 0)
                        {
                            neighborIndex = i - sizex;
                        }
                        break;
                    case 2:
                        if (z < sizez - 1)
                        {
                            neighborIndex = i + sizex * sizey;
                        }
                        break;
                    case 3:
                        if (z > 0)
                        {
                            neighborIndex = i - sizex * sizey;
                        }
                        break;
                    case 4:
                        if (x < sizex - 1)
                        {
                            neighborIndex = i + 1;
                        }
                        break;
                    case 5:
                        if (x > 0)
                        {
                            neighborIndex = i - 1;
                        }
                        break;
                }

                if (neighborIndex >= 0)
                {
                    neighborSolid = newBlockData[neighborIndex].isSolid;
                }


                //marker4n.End();

                //marker41.Begin();

                if (!neighborSolid)
                {
                    // add the face to the mesh


                    


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

                    verts.Add(vertsout0);
                    verts.Add(vertsout1);
                    verts.Add(vertsout2);
                    verts.Add(vertsout3);

                    tris.Add(_triangleVertices[face, 0] + vertIndex);
                    tris.Add(_triangleVertices[face, 1] + vertIndex);
                    tris.Add(_triangleVertices[face, 2] + vertIndex);
                    tris.Add(_triangleVertices[face, 3] + vertIndex);
                    tris.Add(_triangleVertices[face, 4] + vertIndex);
                    tris.Add(_triangleVertices[face, 5] + vertIndex);


                    vertIndex += 4;
                    triIndex += 6;

                    

                    

                }
                //marker41.End();

                //marker4.End();
            }
            //marker3.End();

            //markerInMain.End();
        }

        

        counts[0] = vertIndex;
        counts[1] = triIndex;


        //markerPreMain.End();

        //markerFull.End();
    }

    /// <summary>
    /// The isNeighborSolid.
    /// </summary>
    /// <param name="blockData">The blockData<see cref="NativeArray{BlockData}"/>.</param>
    /// <param name="index">The index<see cref="int"/>.</param>
    /// <param name="face">The face<see cref="int"/>.</param>
    /// <returns>The <see cref="Boolean"/>.</returns>
    private Boolean isNeighborSolid(NativeArray<BlockData> blockData, int index, int face)
    {

        ProfilerMarker marker = new ProfilerMarker("IsNeighbor");
        marker.Begin();
        int neighborIndex = -1;


        switch (face)
        {
            case 0:
                if (y < sizey - 1)
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

        marker.End();

        if (neighborIndex < 0)
        {
            return false;
        }

        return blockData[neighborIndex].isSolid;
    }
}
