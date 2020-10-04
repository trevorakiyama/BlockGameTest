using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
using Unity.Profiling;

public class ChunkManagerUnityUtils
{


    


    public void CreateMeshesForChunks(Dictionary<int3, Chunk> chunkMap, int3 chunkSize, ChunkManager chunkMgr, ChunkRenderer chunkRenderer)
    {
        ProfilerMarker m = new ProfilerMarker("CreateMeshesForChunkspub");
        ProfilerMarker m1 = new ProfilerMarker("1CreateMeshesForChunkspub");
        ProfilerMarker m2 = new ProfilerMarker("2CreateMeshesForChunkspub");
        m.Begin();

       
        List<Chunk> chunksToRender = new List<Chunk>();


        foreach (var kvp in chunkMap.ToList())
        {
            if (kvp.Value.dataInitialized || kvp.Value.recalculateMesh)
            {
                chunksToRender.Add(kvp.Value);
            }
        }

        List<Chunk> chunksRendered = CreateMeshesForChunks(chunksToRender, chunkSize, chunkMgr, chunkRenderer);


        for (int i = 0; i < chunksRendered.Count; i++)
        {
            Chunk chunk = chunksRendered[i];
            chunk.recalculateMesh = false;
            chunkMap.Remove(chunk.pos);
            chunkMap.Add(chunk.pos, chunk);
        }

        m.End();

    }


    private List<Chunk> CreateMeshesForChunks(List<Chunk> chunks, int3 chunkSize, ChunkManager chunkMgr, ChunkRenderer chunkRenderer)
    {
        ProfilerMarker m = new ProfilerMarker("CreateMeshesForChunks");
        ProfilerMarker m1 = new ProfilerMarker("1CreateMeshesForChunks");
        ProfilerMarker m11 = new ProfilerMarker("11CreateMeshesForChunks");
        ProfilerMarker m12 = new ProfilerMarker("12CreateMeshesForChunks");
        ProfilerMarker m2 = new ProfilerMarker("2CreateMeshesForChunks");
        m.Begin();
        m1.Begin();
        // TODO: Move this to Unity Specific Code?
        // TODO: Create the Meshes
        // 1 Get the ChunkBlockData
        // 2 Call the Chunk Renderer to render the chunk based on map of ChunkBlockData and the ChunkMetaData


        // Phase 1:  Render one at a time (as above)
        // Phase 2:  Render in a batch

        List <Chunk> chunksUpdated = new List<Chunk>();

        List<NativeArray<ChunkBlockData>> multiChunkBlockData = new List<NativeArray<ChunkBlockData>>();
        List<Chunk> chunksToRecalculate = new List<Chunk>();



        


        for (int i = 0; i < chunks.Count; i++)
        {
            Chunk chunk = chunks[i];

            if (chunk.recalculateMesh)
            {
                m11.Begin();

                NativeArray<ChunkBlockData> singleChunkBlockData = new NativeArray<ChunkBlockData>(chunkSize.x * chunkSize.y * chunkSize.z, Allocator.TempJob);

                m11.End();

                m12.Begin();


                chunkMgr.getChunkBlockData(chunk.pos, ref singleChunkBlockData, false);

                

                multiChunkBlockData.Add(singleChunkBlockData);
                chunksToRecalculate.Add(chunk);

                m12.End();

                // TODO:  Make this really a batch operation
                //chunkRenderer.generateMeshesForChunks(chunkSize, chunk.pos, singleChunkBlockDataB, chunk.meshFilter, chunk.chunkObject.transform);

                //chunk.recalculateMesh = false;
                //chunksToUpdate.Add(chunk);
            }
        }

        m1.End();

        m2.Begin();

        chunkRenderer.generateMeshesForChunks(chunkSize, chunksToRecalculate, multiChunkBlockData);




        for (int i = 0; i < multiChunkBlockData.Count; i++)
        {

            multiChunkBlockData[i].Dispose();

        }


        m2.End();
        m.End();

        return chunksToRecalculate;

    }




}
