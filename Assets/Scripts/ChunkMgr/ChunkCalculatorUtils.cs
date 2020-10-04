using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class ChunkCalculatorUtils
{

    NativeArray<int3> nearestLocationsFromOrigin;



    public ChunkCalculatorUtils(ChunkManager chunkManager, int maxDist)
    {

        NativeList<int3> coordList = new NativeList<int3>(maxDist * maxDist, Allocator.Temp);

        for (int x = 0; x <= maxDist * 2 ; x++)
        {
            for (int z = 0; z <= maxDist * 2 ; z++)
            {
                coordList.Add(new int3(x - maxDist , 0, z - maxDist ));
            }
        }

        coordList.Sort<int3, VecCompare>(new VecCompare());

        nearestLocationsFromOrigin = new NativeArray<int3>(coordList.Length, Allocator.Persistent);
        nearestLocationsFromOrigin.CopyFrom(coordList);

        coordList.Dispose();
    }

    public void Dispose()
    {
        nearestLocationsFromOrigin.Dispose();
    }






    // TODO: Move this somewhere else
    public int3[] OrderedNearestChunkPos(int3 origin, int minDist, int maxDist, int maxCount)
    {
        NativeList<int3> orderedCoords = new NativeList<int3>(maxDist * maxDist, Allocator.TempJob);


        new NearestRelativeChunks()
        {
            _allChunks = nearestLocationsFromOrigin,
            _filteredChunks = orderedCoords,
            _minDist = minDist,
            _maxDist = maxDist,
            _origin = origin,
            _maxCount = maxCount/2
        }.Run();



        int3[] orderedCoordsArray = orderedCoords.ToArray();

        orderedCoords.Dispose();

        return orderedCoordsArray;
    }




}


[BurstCompile]
public  struct NearestRelativeChunks : IJob
{

    public NativeArray<int3> _allChunks;
    public NativeList<int3> _filteredChunks;


    public int _minDist;
    public int _maxDist;
    public int _maxCount;
    public int3 _origin;



    void IJob.Execute()
    {

        int maxDist2 = _maxDist * _maxDist;
        int minDist2 = _minDist * _minDist;


        for (int i = 0; i < _allChunks.Length && i < _maxCount; i++)
        {
            int mag2 = _allChunks[i].x * _allChunks[i].x + _allChunks[i].z * _allChunks[i].z;

            if (mag2 >= minDist2 && mag2 <= maxDist2)
            {
                _filteredChunks.Add(_allChunks[i] + _origin);
            }
        }
    }

}
