using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;
using Unity.Profiling;
using Unity.Collections.LowLevel.Unsafe;


// Handles The Chunk Logic
// Currently only the terrain logic
public class ChunkManager {


    public World myWorld;

    public static int3 chunkSize = new int3(16, 16, 16);

    ChunkMeshes chunkMeshes;


    // TODO:  Maybe these should go onto a Unity Object so they can be safely cleaned up when the program closes

    NativeArray<BlockTypeData> blockTypeData;
    NativeHashMap<int3, ChunkData> chunkData;

    NativeArray<ChunkMeshVertexData> _verts;
    NativeArray<int> _tris;



    // TODO: Perhaps this should be turned into a NativeMap or even Entities?
    internal Dictionary<int3, Chunk> tempChunks = new Dictionary<int3, Chunk>();


    ProfilerMarker m1 = new ProfilerMarker("m1");
    ProfilerMarker m3 = new ProfilerMarker("m3");

    public ChunkManager(World myWorld)
    {

        this.myWorld = myWorld;

        // TODO: Make this a real initizer rather than 
        // Hard code 2 block types for now:

        blockTypeData = new NativeArray<BlockTypeData>(1, Allocator.Persistent);
        chunkData = new NativeHashMap<int3, ChunkData>(1024, Allocator.Persistent);


        int3 chunkSize = ChunkManager.chunkSize;

        _verts = new NativeArray<ChunkMeshVertexData>(chunkSize.x * chunkSize.y * chunkSize.z * 24, Allocator.Persistent);
        _tris = new NativeArray<int>(chunkSize.x * chunkSize.y * chunkSize.z * 36, Allocator.Persistent);



        chunkMeshes = new ChunkMeshes(chunkSize);

    }
        

    public void Dispose()
    {
        blockTypeData.Dispose();
        chunkData.Dispose();
        chunkMeshes.Dispose();

        _verts.Dispose();
        _tris.Dispose();
    }
    



    public void checkUpdates()
    {

    }


    public void LoadChunks()
    {


    }


    public Boolean getChunkBlockData(int3 chunkCoord, ref NativeArray<ChunkBlockData> singleChunkBlockData)
    {

        ChunkLoader.LoadChunk(singleChunkBlockData, chunkCoord, ChunkManager.chunkSize);

        return true;
    }


    int lastChunksToKeep = 0;



    // TODO:  This is doing too much.  Initializing AND Generating the meshes
    internal void ProcessChunksMeshes(int3 chunkPos)
    {

        ProfilerMarker p1 = new ProfilerMarker("p1");
        ProfilerMarker p2 = new ProfilerMarker("p2");
        ProfilerMarker p3 = new ProfilerMarker("p3");
        ProfilerMarker p4 = new ProfilerMarker("p4");


        int3 chunkSize = ChunkManager.chunkSize;

        int maxDist = 24;

        // Make the Circle based on the player position, not just the chunk

        NativeList<int3> nearChunks = new NativeList<int3>(0, Allocator.TempJob);


        int3 chunkCoord = new int3(chunkPos.x, chunkPos.y, chunkPos.z);

        CircularSearchJob job = new CircularSearchJob();
        job.orderedCoords = nearChunks;
        job.origin = chunkCoord;
        job.maxDist = maxDist;
        var handle = job.Schedule();

        handle.Complete();


        long timestamp = System.Diagnostics.Stopwatch.GetTimestamp();


        p2.Begin();

        for (int i = 0; i < job.orderedCoords.Length; i++)
        {
            int3 vcoord = job.orderedCoords[i];


            Chunk foundChunk;
            bool found = tempChunks.TryGetValue(vcoord, out foundChunk);


            if (!found)
            {

                int3 coord = new int3(vcoord.x, vcoord.y, vcoord.z);


                // TODO:  need a better way to deal with this because it get's disposed automatically

                if (System.Diagnostics.Stopwatch.GetTimestamp() - timestamp < 50000L)
                {
                    m1.Begin();

                    NativeArray<ChunkBlockData> singleChunkBlockData = new NativeArray<ChunkBlockData>(chunkSize.x * chunkSize.y * chunkSize.z, Allocator.TempJob);
                    getChunkBlockData(coord, ref singleChunkBlockData);


                    foundChunk = new Chunk(myWorld.textureMaterials, vcoord, this);
                    m1.End();

                    tempChunks.Add(coord, foundChunk);
                    m1.Begin();

                    chunkMeshes.generateMeshesForChunks(chunkSize, coord, singleChunkBlockData, foundChunk.meshFilter, foundChunk.chunkObject.transform);


                    m1.End();

                    singleChunkBlockData.Dispose();

                    //break;
                }

            }
        }

        p2.End();


        // dispose of all chunks that are not in th local range
        // Should probably sort these as well



        //// THIS IS BRUTALLY SLOW AND TAKING ALL THE TIME
        ///THOUGHTS: Loop through once and mereley check against distance from player
        ///Also turn off the mesh generation unless needed

        p3.Begin();

        // Convert the Map of chunks to a list

        List<Chunk> allChunks = new List<Chunk>();
        foreach (var kvp in tempChunks)
        {
            allChunks.Add(kvp.Value);
        }

        var filteredChunks = FilterChunksFurtherThan(maxDist, chunkCoord, allChunks);

        foreach (Chunk chunk in filteredChunks)
        {

            //kvp.Value.chunkObject.transform.position = new float3(10000, 0, 0);
            //kvp.Value.meshFilter.mesh.Clear();

            disposeChunk(chunk);
            tempChunks.Remove(chunk.pos);
        }



        nearChunks.Dispose();

        p3.End();


    }


    protected List<Chunk> FilterChunksFurtherThan(int maxDist, int3 origin, List<Chunk> chunks)
    {


        List<Chunk> filteredChunks = new List<Chunk>();


        foreach (Chunk chunk in chunks)
        {
            long maxDistSquare =
                maxDist * maxDist;

            int3 pos = chunk.pos;

            long distSquare =
                (origin.x - chunk.pos.x) * (origin.x - chunk.pos.x)
                + (origin.y - chunk.pos.y) * (origin.y - chunk.pos.y)
                + (origin.z - chunk.pos.z) * (origin.z - chunk.pos.z);



            if (maxDistSquare < distSquare)
            {
                filteredChunks.Add(chunk);
            }

        }

        return filteredChunks;
    }




    private void disposeChunk(Chunk chunk)
    {

        myWorld.DestroyObject(chunk.chunkObject);


    }




    //unsafe void generateMeshesForChunks(int3 chunkSize, int3 chunkCoord, NativeArray<ChunkBlockData> blockData, MeshFilter meshFilter, Transform transform)
    //{

    //    ProfilerMarker marker = new ProfilerMarker("GenerateMeshes");
    //    ProfilerMarker m1 = new ProfilerMarker("g1");
    //    ProfilerMarker m2 = new ProfilerMarker("g2"); 
    //    ProfilerMarker m3 = new ProfilerMarker("g3");
    //    ProfilerMarker m31 = new ProfilerMarker("g31");
    //    ProfilerMarker m4 = new ProfilerMarker("g4");
    //    ProfilerMarker m5 = new ProfilerMarker("g5");


    //    marker.Begin();



    //    /// Each chunk input
    //    /// - Chunk Coordinate
    //    /// - BlockData
    //    /// - Chunk size
    //    /// Each chunk output
    //    /// - Vertex List
    //    /// - Triangle List


    //    /// For a batch
    //    /// Input
    //    /// - List of Chunk Coordinates
    //    /// - List of Pointers to BlockData
    //    /// - Chunk Size
    //    /// Each Chunk Output
    //    /// - List of pointers to Vertex List
    //    /// - List of pointers to Triangle List
    //    /// - List of Counts of Vertex List
    //    /// - List of Counts of Triangle List
    //    /// 



    //    // TEMPORARY: TODO Convert the single data into a multi data form


    //    m1.Begin();

    //    // Inputs
    //    NativeArray<ulong> _blockDataPtrs = new NativeArray<ulong>(1, Allocator.TempJob);
    //    _blockDataPtrs[0] = (ulong)blockData.GetUnsafePtr();

    //    NativeArray<int> _blockDataCounts = new NativeArray<int>(1, Allocator.TempJob);
    //    _blockDataCounts[0] = blockData.Length;

    //    NativeArray<int3> _chunkSize = new NativeArray<int3>(1, Allocator.TempJob);
    //    _chunkSize[0] = chunkSize;



    //    // Outputs

    //    NativeArray<ulong> _vertsPtrs = new NativeArray<ulong>(1, Allocator.TempJob);
    //    NativeArray<int> _vertsCount = new NativeArray<int>(1, Allocator.TempJob);
    //    NativeArray<ulong> _trisPtrs = new NativeArray<ulong>(1, Allocator.TempJob);
    //    NativeArray<int> _trisCount = new NativeArray<int>(1, Allocator.TempJob);

    //    m1.End();


    //    _vertsPtrs[0] = (ulong)_verts.GetUnsafePtr();
    //    _trisPtrs[0] = (ulong)_tris.GetUnsafePtr();

    //    m2.Begin();

    //    // Operate
    //    generateMeshesForChunks(ref _blockDataPtrs, ref _blockDataCounts, ref _chunkSize,
    //        ref _vertsPtrs, ref _vertsCount, ref _trisPtrs, ref _trisCount);

    //    m2.End();
    //    m31.Begin();


    //    // Index is a temp way to do one chunk
    //    int index = 0;


    //    Mesh mesh = new Mesh();
    //    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

    //    m31.End();

    //    m3.Begin();


    //    // TODO:  This could probably be put somewhere else
    //    var layout = new[]
    //    {
    //        new UnityEngine.Rendering.VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
    //        new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
    //        new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
    //    };
        

    //    mesh.SetVertexBufferParams(_vertsCount[index], layout);
    //    mesh.SetVertexBufferData(_verts, 0, 0, _vertsCount[index]);

    //    m3.End();
    //    m4.Begin();


    //    NativeSlice<int> slice = _tris.Slice<int>(0,_trisCount[index]);
    //    mesh.triangles = slice.ToArray();

    //    m4.End();

    //    Bounds bounds = new Bounds();
    //    bounds.min = new Vector3(0, 0, 0);
    //    bounds.max = new Vector3(chunkSize.x, chunkSize.y, chunkSize.z);

    //    m5.Begin();

    //    mesh.bounds = bounds;


    //    meshFilter.mesh = mesh;
    //    transform.position = new Vector3(chunkSize.x * chunkCoord.x, 0, chunkSize.z * chunkCoord.z);

    //    _blockDataPtrs.Dispose();
    //    _blockDataCounts.Dispose();
    //    _chunkSize.Dispose();

    //    _vertsPtrs.Dispose();
    //    _vertsCount.Dispose();
    //    _trisPtrs.Dispose();
    //    _trisCount.Dispose();

    //    m5.End();
       

    //    marker.End();

    //}



    //// TODO:  This should be moved out of here
    //protected void generateMeshesForChunks(
    //    ref NativeArray<ulong> _blockDataPtrs,
    //    ref NativeArray<int> _blockDataCounts,
    //    ref NativeArray<int3> _chunkSize,
    //    ref NativeArray<ulong> _vertsPtrs,
    //    ref NativeArray<int> _vertCount,
    //    ref NativeArray<ulong> _trisPtrs,
    //    ref NativeArray<int> _trisCount
    //    )
    //{


    //    JobHandle handle = new MultiMeshCreateJob()
    //    {
    //        blockDataPtrs = _blockDataPtrs,
    //        bockDataCounts = _blockDataCounts,
    //        chunkSize = _chunkSize,
    //        vertsPtrs = _vertsPtrs,
    //        vertCount = _vertCount,
    //        trisPtrs = _trisPtrs,
    //        triIntCounts = _trisCount
    //    }.Schedule(_blockDataPtrs.Length, 1);

    //    handle.Complete();

    //}

}






public struct ChunkData
{
    public Boolean isValid;
    public Boolean isInitialized;
    public int xcoord;
    public int ycoord;
    public int chunkBlockData;

}


public struct ChunkBlockData
{
    public Boolean isVisible;
    public Boolean isSolid;
    public uint blockTypeId;
}

public struct BlockTypeData
{
    public Boolean isVisible;
    public Boolean isSolid;
    public byte direction;
    public uint2 uvTop;
    public uint2 uvBottom;
    public uint2 uvNorth;
    public uint2 uvSouth;
    public uint2 uvEast;
    public uint2 uvWest;

}

