using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine.Rendering;


// Handles The Chunk Logic
// Currently only the terrain logic
public class ChunkManager {


    public World myWorld;

    // TODO:  Maybe these should go onto a Unity Object so they can be safely cleaned up when the program closes

    NativeArray<BlockTypeData> blockTypeData;
    NativeHashMap<int3, ChunkData> chunkData;


    internal Dictionary<Vector3Int, Chunk> tempChunks = new Dictionary<Vector3Int, Chunk>();

    public int dummyValue;


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
        //chunkBlockData.Dispose();
    }
    



    public void checkUpdates()
    {
        //ChunkBlockData[] managedChunkData;
        //ChunkLoader.LoadChunk(new int3(0, 0,0), new uint3(16, 16, 16), out managedChunkData);

        //dummyValue += 1;
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

    internal void ProcessChunksMeshes(int3 chunkPos)
    {


        // Make the Circle based on the player position, not just the chunk

        NativeList<Vector3Int> nearChunks = new NativeList<Vector3Int>(0, Allocator.TempJob);

        Vector3Int chunkCoord = new Vector3Int(chunkPos.x, chunkPos.y, chunkPos.z);

        CircularSearchJob job = new CircularSearchJob();
        job.orderedCoords = nearChunks;
        job.origin = chunkCoord;
        job.maxDist = 64;
        var handle = job.Schedule();

        handle.Complete();


        long timestamp = System.Diagnostics.Stopwatch.GetTimestamp();


        for (int i = 0; i < job.orderedCoords.Length; i++)
        {
            Vector3Int vcoord = job.orderedCoords[i];



            Chunk foundChunk;
            bool found = tempChunks.TryGetValue(vcoord, out foundChunk);


            if (!found)
            {

                int3 coord = new int3(vcoord.x, vcoord.y, vcoord.z);

                NativeArray<ChunkBlockData> singleChunkBlockData = new NativeArray<ChunkBlockData>(Chunk.chunkWidth * Chunk.chunkHeight * Chunk.chunkWidth, Allocator.TempJob);
                getChunkBlockData(coord, ref singleChunkBlockData);

                foundChunk = new Chunk(myWorld, vcoord, this);
                tempChunks.Add(vcoord, foundChunk);


                generateMeshForChunk(coord, singleChunkBlockData, foundChunk.meshFilter, foundChunk.chunkObject.transform);

                //singleChunkBlockData.Dispose();

            }


            //foundChunk.renderChunk(new Vector3(currChunk.x * Chunk.chunkWidth, currChunk.y * Chunk.chunkHeight, currChunk.z * Chunk.chunkWidth));

            if (System.Diagnostics.Stopwatch.GetTimestamp() - timestamp > 200000l)
            {
                break;
            }




            //// check if the chunk is already in Chunkd, if it is not, then generate it, otherwise skip it

            //Vector3Int currChunk = job.orderedCoords[i];

            //Chunk foundChunk;
            //bool found = chunksd.TryGetValue(currChunk, out foundChunk);

            //if (!found)
            //{

            //    Debug.LogFormat("Chunk NEW Pos {0} {1}", chunkCoord.ToString(), playerPosition);

            //    foundChunk = new Chunk(this, currChunk, chunkManager);

            //    //foundChunk.initializeChunkData();
            //    chunksd.Add(currChunk, foundChunk);

            //}


            //foundChunk.renderChunk(new Vector3(currChunk.x * Chunk.chunkWidth, currChunk.y * Chunk.chunkHeight, currChunk.z * Chunk.chunkWidth));

            //if (System.Diagnostics.Stopwatch.GetTimestamp() - timestamp > 100000)
            //{
            //    break;
            //}



        }



        nearChunks.Dispose();
    }



    void generateMeshForChunk(int3 chunkCoord, NativeArray<ChunkBlockData> blockData, MeshFilter meshFilter, Transform transform)
    {


        NativeArray<int> returnCounts = new NativeArray<int>(2, Allocator.TempJob);


        NativeList<ChunkMeshVertexData> meshVertices = new NativeList<ChunkMeshVertexData>(0, Allocator.TempJob);
        NativeList<int> triVerts = new NativeList<int>(0, Allocator.TempJob);

        //NativeArray<ChunkBlockData> lblockData = new NativeArray<ChunkBlockData>(Chunk.chunkWidth * Chunk.chunkHeight * Chunk.chunkWidth, Allocator.TempJob);


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




    public void buildMeshes()
    {

    }

    public void buildChunkData()
    {


        // get all the chunks that need meshes rebuilt


        // Loop through them all and generate mesh data

        // rebuild the meshes


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