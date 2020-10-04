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
using System.Linq;


// Handles The Chunk Logic
// Currently only the terrain logic
public class ChunkManager {


    public World myWorld;

    public static int3 chunkSize = new int3(16, 16, 16);

    ChunkRenderer chunkRenderer;
    ChunkLoader chunkLoader;
    ChunkCalculatorUtils chunkCalcUtils;
    ChunkManagerUnityUtils unityUtils;

    NativeArray<ChunkMeshVertexData> _verts;
    NativeArray<int> _tris;



    // TODO: Perhaps this should be turned into a NativeMap or even Entities?
    internal Dictionary<int3, Chunk> tempChunks = new Dictionary<int3, Chunk>();


    ProfilerMarker m1 = new ProfilerMarker("m1");
    ProfilerMarker m3 = new ProfilerMarker("m3");

    public ChunkManager(World myWorld)
    {

        this.myWorld = myWorld;

        _verts = new NativeArray<ChunkMeshVertexData>(chunkSize.x * chunkSize.y * chunkSize.z * 24, Allocator.Persistent);
        _tris = new NativeArray<int>(chunkSize.x * chunkSize.y * chunkSize.z * 36, Allocator.Persistent);


        chunkRenderer = new ChunkRenderer(chunkSize);
        chunkLoader = new ChunkLoader(chunkSize);
        chunkCalcUtils = new ChunkCalculatorUtils(this, World.retentionDistance);

        unityUtils = new ChunkManagerUnityUtils();

    }
        

    public void Dispose()
    {
        chunkRenderer.Dispose();
        chunkLoader.Dispose();
        chunkCalcUtils.Dispose();

        _verts.Dispose();
        _tris.Dispose();

    }
    



    public void checkUpdates()
    {

    }


    public void LoadChunks()
    {


    }








    public bool getChunkBlockData(int3 chunkCoord, ref NativeArray<ChunkBlockData> singleChunkBlockData, bool loadIfNotLoaded)
    {
        ProfilerMarker m = new ProfilerMarker("getChunkBlockData");
        m.Begin();

        bool returnVal = chunkLoader.GetChunkData(singleChunkBlockData, chunkCoord, loadIfNotLoaded);

        m.End();
        return returnVal;

    }


    int lastChunksToKeep = 0;



    internal void ProcessChunksMeshes(int3 chunkPos, int3 renderDistance)
    {
        

       


    }






    // TODO:  This is doing too much.  Initializing AND Generating the meshes
    internal void ProcessChunksMeshesNew(int3 chunkPos)
    {

        ProfilerMarker p1 = new ProfilerMarker("p1");
        ProfilerMarker p2 = new ProfilerMarker("p2");
        ProfilerMarker p3 = new ProfilerMarker("p3");
        ProfilerMarker p4 = new ProfilerMarker("p4");


        int3 chunkSize = ChunkManager.chunkSize;

        int maxDist = World.renderDistance;

        // Make the Circle based on the player position, not just the chunk

        //NativeList<int3> nearChunks = new NativeList<int3>(0, Allocator.TempJob);


        int3 chunkCoord = new int3(chunkPos.x, chunkPos.y, chunkPos.z);


        int3[] nearestChunks = chunkCalcUtils.OrderedNearestChunkPos(chunkCoord, 0, maxDist, 10000);


        //int3[] nearestChunks = OrderedNearestChunkPos(chunkCoord, maxDist);



        long timestamp = System.Diagnostics.Stopwatch.GetTimestamp();

        // TUrn this into two lists

        p1.Begin();

        for (int i = 0; i < nearestChunks.Length; i++)
        {
            int3 vcoord = nearestChunks[i];


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
                    getChunkBlockData(coord, ref singleChunkBlockData, true);
                    foundChunk = new Chunk(myWorld.textureMaterials, vcoord, this);
                    foundChunk.dataInitialized = true;
                    foundChunk.recalculateMesh = true;
                    m1.End();

                    tempChunks.Add(coord, foundChunk);

                    singleChunkBlockData.Dispose();
               }

            }
        }

        p1.End();

        p2.Begin();


        // TODO:  This can now be called at any time not necessarily in this method.
        unityUtils.CreateMeshesForChunks(tempChunks, chunkSize, this, chunkRenderer);


        p2.End();


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



        //nearChunks.Dispose();

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



    // TODO: Move this somewhere else
    public int3[] OrderedNearestChunkPos(int3 origin, int maxDist)
    {
        NativeList < int3 > orderedCoords = new NativeList<int3>(maxDist * maxDist, Allocator.TempJob);

        new CircularSearchJob()
        {
            _origin = origin,
            _maxDist = maxDist,
            _orderedCoords = orderedCoords
        }.Run();

        int3[] posArray = orderedCoords.ToArray();

        orderedCoords.Dispose();

        return posArray;
    }





    private void disposeChunk(Chunk chunk)
    {

        myWorld.DestroyObject(chunk.chunkObject);
       


    }

}




public struct ChunkBlockData
{
    public Boolean isVisible;
    public Boolean isSolid;
    public uint blockTypeId;
}
