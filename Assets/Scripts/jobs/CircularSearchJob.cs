using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct CircularSearchJob : IJob
{

    public NativeList<int3> orderedCoords;


    public int3 origin;
    public int maxDist;


    void IJob.Execute()
    {

        int maxDist2 = maxDist * maxDist;

        for (int x = -maxDist; x < maxDist; x++)
        {
            for (int z = -maxDist; z < maxDist; z++)
            {

                if (x *x +  z * z <= maxDist2)
                {
                    orderedCoords.Add(new int3(x , 0, z));

                }

            }

        }

        
        // TODO: Could chain the sorting and readjustment as another job but do that later if there's a problem
        orderedCoords.Sort<int3, VecCompare>(new VecCompare());


        for (int i = 0; i < orderedCoords.Length; i++)
        {
            int3 coord = orderedCoords[i] + origin;
            orderedCoords[i] = coord;

        }

    }
}

public readonly struct VecCompare : IComparer<int3>
{
    public int Compare(int3 v1, int3 v2)
    {

        int v1mag = v1.x * v1.x + v1.y * v1.y + v1.z * v1.z;
        int v2mag = v2.x * v2.x + v2.y * v2.y + v2.z * v2.z;

        if (v1mag < v2mag)
        {
            return -1;
        }
        if (v1mag > v2mag)
        {
            return 1;
        }

        return 0;
    }
}


