
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;



public class ChunkLoader
{

    public static void LoadChunk(NativeArray<ChunkBlockData> chunkData, int3 chunkCoord, int3 chunkSize)
    {

        // Has the chunk been saved to disk?
        // if so then load it
        // if not then generate it

        // for not always generate it

        GenerateChunk(chunkData, chunkCoord, chunkSize);
    }


    public void intializeChunks(int3 center, int radius)
    {
        // Check for any chunks that aren't initialized in range and initialize







    }



    private static void GenerateChunk(NativeArray<ChunkBlockData> chunkData, int3 chunkCoord, int3 chunkSize)
    {
        ProfilerMarker marker1 = new ProfilerMarker("ChunkLoader.loader");


        marker1.Begin();

        
        //NativeList<ChunkBlockData> _blockDataOut = new NativeList<ChunkBlockData>((int) (chunkSize.x * chunkSize.y * chunkSize.z), Allocator.TempJob);
        NativeArray<int3> _chunkSizeInput = new NativeArray<int3>(1, Allocator.TempJob);

        _chunkSizeInput[0] = chunkSize;




        JobHandle handle = new generateChunkJob()
        {

            blockDataOut = chunkData,
            chunkSizeInput = _chunkSizeInput
        }.Schedule();


        handle.Complete();

        //_blockDataOut.Dispose();
        _chunkSizeInput.Dispose();
        marker1.End();
        
    }


}


[Unity.Burst.BurstCompile]
public struct generateChunkJob : IJob
{

    public NativeArray<ChunkBlockData> blockDataOut;
    public NativeArray<int3> chunkSizeInput;

    public void Execute()
    {
        int3 chunkSize = chunkSizeInput[0];


        //ChunkBlockData[] chunkData = new ChunkBlockData[chunkSize.x * chunkSize.y * chunkSize.z];


        int i = 0;

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

                    blockDataOut[i++] = chunkBlockData;
                }
            }
        }







    }
}
