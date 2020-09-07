﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.Rendering;

/// <summary>
/// Class <c>Chunk</c> represents a chunk of block voxels for managmeent and rendering
/// </summary>
public class Chunk
{
    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    World world;
    Vector3Int pos;



    // A chunk will contain:
    // A mesh object for manipulation and rendering
    // an 3d array representing all the block in the chunk

    // A chunk can return a mesh for rendering

    public const int chunkWidth = 32;
    public const int chunkHeight = 256;



    public NativeArray<BlockData> blockData = new NativeArray<BlockData>(chunkWidth * chunkWidth * chunkHeight, Allocator.Persistent);



    Boolean dataInitialized = false;
    Boolean recalculateMesh = true;

    static ProfilerMarker marker = new ProfilerMarker("MyMarker");
    static ProfilerMarker marker2 = new ProfilerMarker("MyMarker2");
    static ProfilerMarker marker3 = new ProfilerMarker("MyMarkerJobPart");
    public static ProfilerMarker marker1 = new ProfilerMarker("Const 1");


    public Chunk(World world, Vector3Int pos)
    {

        marker1.Begin();
        this.world = world;
        this.pos = pos;
        chunkObject = new GameObject();
        chunkObject.name = "This Named Chunk" + pos;
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        //meshRenderer.material = world.textureMaterials;
        meshRenderer.material = world.textureMaterials;

        marker1.End();


        recalculateMesh = true;
    }

    public void renderChunk(Vector3 offset)
    {
        if (recalculateMesh)
        {
            Mesh mesh = GetMesh();
            meshFilter.mesh = mesh;

            chunkObject.transform.position = offset;

            recalculateMesh = false;
        }
        
    }

    

    public Mesh GetMesh()
    {

        if (dataInitialized == false)
        {
            initializeChunkData();
            dataInitialized = true;
        }

        return BuildMesh();
    }





    public void initializeChunkData()
    {
        marker2.Begin();
        // Simple 


        ChunkInitJob job = new ChunkInitJob();
        job.chunkHeight = chunkHeight;
        job.chunkWidth = chunkWidth;
        job.result = blockData;
        JobHandle handle = job.Schedule();

        handle.Complete();

        marker2.End();

    }



    private BlockData getBlockData(Vector3Int blockCoord)
    {
        int index = blockCoord.x + blockCoord.x * blockCoord.y + blockCoord.x * blockCoord.y * blockCoord.z;

        return blockData[index];

    }



    public Mesh BuildMesh()
    {


        marker.Begin();

        marker3.Begin();


        NativeArray<int> returnCounts = new NativeArray<int>(2, Allocator.TempJob);

        NativeList<ChunkMeshVertexData> meshVertices = new NativeList<ChunkMeshVertexData>(0, Allocator.TempJob);
        NativeList<int> triVerts = new NativeList<int>(0, Allocator.TempJob);



        MeshCreateJob job = new MeshCreateJob();
        job.blockData = blockData;
        job.verts = meshVertices;
        job.tris = triVerts;
        job.sizex = chunkWidth;
        job.sizey = chunkHeight;
        job.sizez = chunkWidth;
        job.counts = returnCounts;
        JobHandle handle = job.Schedule();

        marker3.End();

        handle.Complete();


        int vertCount = job.counts[0];
        int triCount = job.counts[1];
        returnCounts.Dispose();


        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
        };


        mesh.SetVertexBufferParams(vertCount, layout);

        mesh.SetVertexBufferData((NativeArray<ChunkMeshVertexData>)job.verts, 0, 0, vertCount);

        int[] trilist = new int[triCount];
        for (int t = 0; t < triCount; t++)
        {
            trilist[t] = job.tris[t];
        }

        mesh.triangles = trilist;

        mesh.RecalculateBounds();


        meshVertices.Dispose();
        triVerts.Dispose();


        marker.End();

        return mesh;
    }




    public static Vector3Int getChunkRelativeCoord(Vector3Int worldCoord)
    {
        Vector3Int offset = new Vector3Int(worldCoord.x % chunkWidth, worldCoord.y % chunkHeight, worldCoord.z % chunkWidth);

        if (offset.x < 0)
        {
            offset.x += chunkWidth;
        }
        if (offset.y < 0)
        {
            offset.y += chunkHeight;
        }
        if (offset.z < 0)
        {
            offset.z += chunkWidth;
        }


        return offset;
    }

    public static Vector3Int getChunkCoord(Vector3Int worldCoord)
    {


        Vector3Int chunkCoord = new Vector3Int(worldCoord.x / chunkWidth, worldCoord.y / chunkHeight, worldCoord.z / chunkWidth);
        if (worldCoord.x < 0)
        {
            chunkCoord.x -= 1;
        }
        if (worldCoord.y < 0)
        {
            chunkCoord.y -= 1;
        }
        if (worldCoord.z < 0)
        {
            chunkCoord.z -= 1;
        }

        return chunkCoord;
    }



    public void cleanup()
    {

        if (blockData.IsCreated)
        {
            
        }
        blockData.Dispose();
    }



}
