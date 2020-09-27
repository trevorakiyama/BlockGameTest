using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct ChunkInitJob : IJob
{
    public int chunkHeight;
    public int chunkWidth;

    public NativeArray<BlockData> result;

    // currently no input


    // Chunk Loading should be a task on a separate Thread
    // Jobs aren't compatible
    void IJob.Execute()
    {

        //BlockData[] resultArr = new BlockData[chunkHeight * chunkWidth * chunkWidth];


        // chunkBlocks = new Block[chunkWidth, chunkHeight, chunkWidth];

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    //int m = chunkHeight - Math.Abs(x + z - 15) / 2;

                    int m = 2;


                    //BlockData data = result[x + y * chunkWidth +  z * chunkHeight * chunkWidth];
                    BlockData data = new BlockData();
                    if (y < m)
                    {

                        //data.name = "testblock";
                        data.isSolid = true;
                        data.isVisible = true;

                    }
                    else
                    {
                        //data.name = "Air";
                        data.isSolid = false;
                        data.isVisible = false;
                    }

                    int calcIndex = x + y * chunkWidth + z * chunkHeight * chunkWidth;

                    result[x + y * chunkWidth + z * chunkHeight * chunkWidth] = data;
                }
            }


        }

    }
}
