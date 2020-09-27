
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;



public class ChunkLoader
{

    public static ChunkBlockData[] LoadChunk(int3 chunkCoord, uint3 chunkSize)
    {

        // Has the chunk been saved to disk?
        // if so then load it
        // if not then generate it

        // for not always generate it

        return GenerateChunk(chunkCoord, chunkSize);
    }


    private static ChunkBlockData[] GenerateChunk(int3 chunkCoord, uint3 chunkSize)
    {
        ProfilerMarker marker1 = new ProfilerMarker("ChunkLoader.loader");


        marker1.Begin();

        
        NativeList<ChunkBlockData> _blockDataOut = new NativeList<ChunkBlockData>((int) (chunkSize.x * chunkSize.y * chunkSize.z), Allocator.TempJob);
        NativeArray<uint3> _chunkSizeInput = new NativeArray<uint3>(1, Allocator.TempJob);

        _chunkSizeInput[0] = chunkSize;




        JobHandle handle = new generateChunkJob()
        {

            blockDataOut = _blockDataOut,
            chunkSizeInput = _chunkSizeInput
        }.Schedule();


        handle.Complete();

        ChunkBlockData[] chunkData = _blockDataOut.ToArray();

        _blockDataOut.Dispose();
        _chunkSizeInput.Dispose();
        marker1.End();
        


        /*
        ChunkBlockData[] chunkData = new ChunkBlockData[chunkSize.x * chunkSize.y * chunkSize.z];

        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y ; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    int index = (int) (x + y * chunkSize.x + z * chunkSize.x * chunkSize.y);

                    float terrainHeight = chunkSize.y - Math.Abs(x + z - 15) / 2;

                    //BlockData data = result[x + y * chunkWidth +  z * chunkHeight * chunkWidth];
                    

                    if (index >= chunkData.Length)
                    {
                        UnityEngine.Debug.Log("OUTOFRANGE");
                    }


                    if (y < terrainHeight)
                    {
                        chunkData[index].blockTypeId = 0;
                        chunkData[index].isSolid = true;
                        chunkData[index].isVisible = true;
                    }
                    else
                    {
                        chunkData[index].blockTypeId = 0;
                        chunkData[index].isSolid = false;
                        chunkData[index].isVisible = false;
                    }
                }
            }
        }

        */


        return chunkData;
    }


}


[Unity.Burst.BurstCompile]
public struct generateChunkJob : IJob
{

    public NativeList<ChunkBlockData> blockDataOut;
    public NativeArray<uint3> chunkSizeInput;

    public void Execute()
    {
        uint3 chunkSize = chunkSizeInput[0];


        //ChunkBlockData[] chunkData = new ChunkBlockData[chunkSize.x * chunkSize.y * chunkSize.z];




        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    int index = (int)(x + y * chunkSize.x + z * chunkSize.x * chunkSize.y);

                    float terrainHeight = chunkSize.y - Math.Abs(x + z - 15) / 2;
                    terrainHeight = 1;
                    //BlockData data = result[x + y * chunkWidth +  z * chunkHeight * chunkWidth];

                    ChunkBlockData chunkBlockData = new ChunkBlockData();


                    if (index >= blockDataOut.Length)
                    {
                        //UnityEngine.Debug.Log("OUTOFRANGE");
                    }


                    if (y < terrainHeight)
                    {
                        chunkBlockData.blockTypeId = 0;
                        chunkBlockData.isSolid = true;
                        chunkBlockData.isVisible = true;
                    }
                    else
                    {
                        chunkBlockData.blockTypeId = 0;
                        chunkBlockData.isSolid = false;
                        chunkBlockData.isVisible = false;
                    }

                    blockDataOut.Add(chunkBlockData);
                }
            }
        }







    }
}
