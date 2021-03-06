﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public struct ChunkInitJob : IJob
{
    public int chunkHeight;
    public int chunkWidth;

    public NativeArray<BlockData> result;

    // currently no input


    void IJob.Execute()
    {



        // chunkBlocks = new Block[chunkWidth, chunkHeight, chunkWidth];

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    int m = 16 - Math.Abs(x + z - 15) / 2;

                    BlockData data = result[x + y * chunkWidth +  z * chunkHeight * chunkWidth];

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
