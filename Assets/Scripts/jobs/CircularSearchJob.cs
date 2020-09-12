using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct CircularSearchJob : IJob
{

    public NativeList<Vector3Int> orderedCoords;

    public Vector3Int origin;
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
                    orderedCoords.Add(new Vector3Int(x , 0, z));

                }

            }

        }

        
        // TODO: Could chain the sorting and readjustment as another job but do that later if there's a problem
        orderedCoords.Sort<Vector3Int, VecCompare>(new VecCompare());


        for (int i = 0; i < orderedCoords.Length; i++)
        {
            Vector3Int coord = orderedCoords[i] + origin;
            orderedCoords[i] = coord;

        }

    }
}

public readonly struct VecCompare : IComparer<Vector3Int>
{
    public int Compare(Vector3Int v1, Vector3Int v2)
    {

        int v1mag = v1.sqrMagnitude;
        int v2mag = v2.sqrMagnitude;
        
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


