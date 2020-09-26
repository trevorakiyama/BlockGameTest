using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;
using UnityEngine;
using Unity.Profiling;


// Handles The Chunk Logic
// Currently only the terrain logic
public class ChunkManager {


    public World myWorld;

    // TODO:  Maybe these should go onto a Unity Object so they can be safely cleaned up when the program closes

    NativeArray<BlockTypeData> blockTypeData;
    NativeHashMap<int3, ChunkData> chunkData;


    internal Dictionary<int3, Chunk> tempChunks = new Dictionary<int3, Chunk>();

    public int dummyValue;


    ProfilerMarker m1 = new ProfilerMarker("m1");
    ProfilerMarker m2 = new ProfilerMarker("m2");
    ProfilerMarker m3 = new ProfilerMarker("m3");
    ProfilerMarker m4 = new ProfilerMarker("m4");


    public ChunkManager(World myWorld)
    {

        this.myWorld = myWorld;

        // TODO: Make this a real initizer rather than 
        // Hard code 2 block types for now:

        blockTypeData = new NativeArray<BlockTypeData>(1, Allocator.Persistent);
        chunkData = new NativeHashMap<int3, ChunkData>(1024, Allocator.Persistent);

        dummyValue = 1;
    }
        

    public void Dispose()
    {
        blockTypeData.Dispose();
        chunkData.Dispose();

        
    }
    



    public void checkUpdates()
    {

    }


    public void LoadChunks()
    {


    }


    public Boolean getChunkBlockData(int3 chunkCoord, ref NativeArray<ChunkBlockData> singleChunkBlockData)
    {

        ChunkBlockData[] chunkData = ChunkLoader.LoadChunk(chunkCoord,
                    new uint3(Chunk.chunkWidth, Chunk.chunkHeight, Chunk.chunkWidth));
        // Terrible copy of copying the data from one type to another 


        singleChunkBlockData.CopyFrom(chunkData);

        return true;
    }


    int lastChunksToKeep = 0;


    internal void ProcessChunksMeshes(int3 chunkPos)
    {


        int maxDist = 64;

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


        Debug.Log($"nearcount = {nearChunks.Length}");


        for (int i = 0; i < job.orderedCoords.Length; i++)
        {
            int3 vcoord = job.orderedCoords[i];


            Chunk foundChunk;
            bool found = tempChunks.TryGetValue(vcoord, out foundChunk);


            if (!found)
            {

                int3 coord = new int3(vcoord.x, vcoord.y, vcoord.z);


                // TODO:  need a better way to deal with this because it get's disposed automatically
               
                


                if (System.Diagnostics.Stopwatch.GetTimestamp() - timestamp < 200000l)
                {
                    NativeArray<ChunkBlockData> singleChunkBlockData = new NativeArray<ChunkBlockData>(Chunk.chunkWidth * Chunk.chunkHeight * Chunk.chunkWidth, Allocator.TempJob);
                    getChunkBlockData(coord, ref singleChunkBlockData);
                    foundChunk = new Chunk(myWorld, vcoord, this);
                    tempChunks.Add(coord, foundChunk);
                    m1.Begin();
                    generateMeshForChunk(coord, singleChunkBlockData, foundChunk.meshFilter, foundChunk.chunkObject.transform);
                    m1.End();
                    //break;
                }

                //generateMeshForChunk(coord, singleChunkBlockData, foundChunk.meshFilter, foundChunk.chunkObject.transform);

            }

            //if (System.Diagnostics.Stopwatch.GetTimestamp() - timestamp > 200000l)
            //{
            //    break;
            //}
        }


        // dispose of all chunks that are not in th local range
        // Should probably sort these as well

        m2.Begin();

        //// THIS IS BRUTALLY SLOW AND TAKING ALL THE TIME
        ///THOUGHTS: Loop through once and mereley check against distance from player
        ///Also turn off the mesh generation unless needed



        // Convert the Map of chunks to a list

        List<Chunk> allChunks = new List<Chunk>();
        foreach(var kvp in tempChunks)
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



        // oldRemoveOld(job.orderedCoords);


        /*

        List<KeyValuePair<int3, Chunk>> chunksToRemove = new List<KeyValuePair<int3, Chunk>>();

        int j = 0;
        int k = 0;
        foreach ( KeyValuePair<int3, Chunk> kvp in tempChunks)
        {

            // todo this needs to be fixed becausae it seems to wipe out all the keys
            
            Boolean found = false;

            for (int i = 0; i < job.orderedCoords.Length; i++)
            {
                if (job.orderedCoords[i].Equals( kvp.Key))
                {

                    k++;
                    found = true;
                    break;
                };
            }

            if (!found)
            {
                chunksToRemove.Add(kvp);
            }

        }



        m2.End();
        m3.Begin();
        Debug.Log($"Chunks to remove {chunksToRemove.Count}");
        Debug.Log($"Chunks to keep {k}");

        if (lastChunksToKeep == chunksToRemove.Count)
        {
            Debug.Log($"removeall");

        }



        lastChunksToKeep = k;

        m3.End();
        m4.Begin();

        foreach (KeyValuePair<int3, Chunk> kvp in chunksToRemove)
        {

            //kvp.Value.chunkObject.transform.position = new float3(10000, 0, 0);
            //kvp.Value.meshFilter.mesh.Clear();

            disposeChunk(kvp.Value);
            tempChunks.Remove(kvp.Key);
        }

        */

        nearChunks.Dispose();

        m4.End();
    }


    protected void oldRemoveOld(NativeList<int3> nearChunkCoords)
    {
        List<KeyValuePair<int3, Chunk>> chunksToRemove = new List<KeyValuePair<int3, Chunk>>();

        int j = 0;
        int k = 0;
        foreach (KeyValuePair<int3, Chunk> kvp in tempChunks)
        {

            // todo this needs to be fixed becausae it seems to wipe out all the keys

            Boolean found = false;

            for (int i = 0; i < nearChunkCoords.Length; i++)
            {
                if (nearChunkCoords[i].Equals(kvp.Key))
                {

                    k++;
                    found = true;
                    break;
                };
            }

            if (!found)
            {
                chunksToRemove.Add(kvp);
            }

        }



        m2.End();
        m3.Begin();
        Debug.Log($"Chunks to remove {chunksToRemove.Count}");
        Debug.Log($"Chunks to keep {k}");

        if (lastChunksToKeep == chunksToRemove.Count)
        {
            Debug.Log($"removeall");

        }



        lastChunksToKeep = k;

        m3.End();
        m4.Begin();

        foreach (KeyValuePair<int3, Chunk> kvp in chunksToRemove)
        {

            //kvp.Value.chunkObject.transform.position = new float3(10000, 0, 0);
            //kvp.Value.meshFilter.mesh.Clear();

            disposeChunk(kvp.Value);
            tempChunks.Remove(kvp.Key);
        }


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

    void generateMeshForChunk(int3 chunkCoord, NativeArray<ChunkBlockData> blockData, MeshFilter meshFilter, Transform transform)
    {


        NativeArray<int> returnCounts = new NativeArray<int>(2, Allocator.TempJob);

        NativeList<ChunkMeshVertexData> meshVertices = new NativeList<ChunkMeshVertexData>(8196, Allocator.TempJob);
        NativeList<int> triVerts = new NativeList<int>(8196, Allocator.TempJob);

        //NativeArray<ChunkBlockData> lblockData = new NativeArray<ChunkBlockData>(Chunk.chunkWidth * Chunk.chunkHeight * Chunk.chunkWidth, Allocator.TempJob);


        m3.Begin();

        // TODO: Investigate mesh generation compution in parallel
        MeshCreateJob job = new MeshCreateJob();
        job.blockData = blockData;
        job.verts = meshVertices;
        job.tris = triVerts;
        job.sizex = Chunk.chunkWidth;
        job.sizey = Chunk.chunkHeight;
        job.sizez = Chunk.chunkWidth;
        job.counts = returnCounts;
        JobHandle handle = job.Schedule();


        handle.Complete();

        m3.End();


        int vertCount = job.counts[0];
        int triCount = job.counts[1];
        returnCounts.Dispose();
        blockData.Dispose();


        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        var layout = new[]
        {
            new UnityEngine.Rendering.VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
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


        meshFilter.mesh = mesh;
        transform.position = new Vector3(Chunk.chunkWidth * chunkCoord.x, 0, Chunk.chunkWidth * chunkCoord.z);
    }

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


public struct testComponent : Unity.Entities.ISystemStateComponentData
{



}