
using System;
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

        marker1.End();

        return chunkData;
    }


}
