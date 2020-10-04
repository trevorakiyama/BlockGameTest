using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class ChunkRenderer
{

    static int chunkCacheArraySize = 100;

    NativeArray<ChunkMeshVertexData>[] _vertsArray = new NativeArray<ChunkMeshVertexData>[chunkCacheArraySize];
    NativeArray<int>[] _trisArray = new NativeArray<int>[chunkCacheArraySize];


    VertexAttributeDescriptor[] _layout = new VertexAttributeDescriptor[]
    {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
    };



    public ChunkRenderer(int3 chunkSize)
    {
        // Set Up the Caches
        for(int i = 0; i < chunkCacheArraySize; i++)
        {
            _vertsArray[i] = new NativeArray<ChunkMeshVertexData>(chunkSize.x * chunkSize.y * chunkSize.z * 24, Allocator.Persistent);
            _trisArray[i] = new NativeArray<int>(chunkSize.x * chunkSize.y * chunkSize.z * 36, Allocator.Persistent);
        }
    }




    // TODO:  This unsafe stuff should be moved into it's own Class

    unsafe public void generateMeshesForChunks(int3 chunkSize, List<Chunk> chunks, List<NativeArray<ChunkBlockData>> blockData)
    {

        ProfilerMarker marker = new ProfilerMarker("GenerateMeshes");
        ProfilerMarker m1 = new ProfilerMarker("g1");
        ProfilerMarker m11 = new ProfilerMarker("g11");
        ProfilerMarker m12 = new ProfilerMarker("g12");
        ProfilerMarker m13 = new ProfilerMarker("g13");
        ProfilerMarker m14 = new ProfilerMarker("g14");
        ProfilerMarker m2 = new ProfilerMarker("g2");
        ProfilerMarker m3 = new ProfilerMarker("g3");
        ProfilerMarker m31 = new ProfilerMarker("g31");
        ProfilerMarker m4 = new ProfilerMarker("g4");
        ProfilerMarker m5 = new ProfilerMarker("g5");


        marker.Begin();



        /// Each chunk input
        /// - Chunk Coordinate
        /// - BlockData
        /// - Chunk size
        /// Each chunk output
        /// - Vertex List
        /// - Triangle List


        /// For a batch
        /// Input
        /// - List of Chunk Coordinates
        /// - List of Pointers to BlockData
        /// - Chunk Size
        /// Each Chunk Output
        /// - List of pointers to Vertex List
        /// - List of pointers to Triangle List
        /// - List of Counts of Vertex List
        /// - List of Counts of Triangle List
        /// 

        m1.Begin();




        int batchSize = chunks.Count < chunkCacheArraySize ? chunks.Count : chunkCacheArraySize;


        // TODO:  If there is a lot of Pointer conversion/passing to jobs, perhaps we need better data structures to keep things clean
        // Inputs to Job
        NativeArray<ulong> _blockDataPtrs = new NativeArray<ulong>(batchSize, Allocator.TempJob);
        NativeArray<int> _blockDataCounts = new NativeArray<int>(batchSize, Allocator.TempJob);
        NativeArray<int3> _chunkSize = new NativeArray<int3>(1, Allocator.TempJob);
        List<Chunk> _chunks = new List<Chunk>();

        // Outputs from Job
        NativeArray<ulong> _vertsPtrs = new NativeArray<ulong>(batchSize, Allocator.TempJob);
        NativeArray<int> _vertsCount = new NativeArray<int>(batchSize, Allocator.TempJob);
        NativeArray<ulong> _trisPtrs = new NativeArray<ulong>(batchSize, Allocator.TempJob);
        NativeArray<int> _trisCount = new NativeArray<int>(batchSize, Allocator.TempJob);


        for (int i = 0; i < batchSize; i++)
        {

            _chunks.Add(chunks[i]);

            // Inputs

            _blockDataPtrs[i] = (ulong)blockData[i].GetUnsafePtr();
            _blockDataCounts[i] = blockData[i].Length;
            _chunkSize[0] = chunkSize;

            _vertsPtrs[i] = (ulong)_vertsArray[i].GetUnsafePtr();
            _trisPtrs[i] = (ulong)_trisArray[i].GetUnsafePtr();

        }


        m1.End();





        m2.Begin();

        if (_blockDataPtrs.Length > 0)
        {
            // Operate
            generateMeshesForChunks(ref _blockDataPtrs, ref _blockDataCounts, ref _chunkSize,
                ref _vertsPtrs, ref _vertsCount, ref _trisPtrs, ref _trisCount);
        }

        m2.End();


        m31.Begin();






        RenderMeshes(
            chunkSize,
            _chunks,
            _vertsPtrs,
            _vertsCount,
            _trisPtrs,
            _trisCount);

        m31.End();





        m4.Begin();

        _blockDataPtrs.Dispose();
        _blockDataCounts.Dispose();
        _chunkSize.Dispose();

        _vertsPtrs.Dispose();
        _vertsCount.Dispose();
        _trisPtrs.Dispose();
        _trisCount.Dispose();

        m4.End();

        marker.End();

    }






    unsafe public void RenderMeshes(
        //int numberOfChunks,
        //MeshFilter meshFilter,
        //Transform transform,
        int3 chunkSize,
        List<Chunk> chunks,
        NativeArray<ulong> _vertsPtrs,
        NativeArray<int> _vertsCount,
        NativeArray<ulong> _trisPtrs,
        NativeArray<int> _trisCount
        )
    {

        ProfilerMarker m = new ProfilerMarker("Allocate");
        ProfilerMarker m1 = new ProfilerMarker("1Allocate");
        ProfilerMarker m11 = new ProfilerMarker("11Allocate");
        ProfilerMarker m12 = new ProfilerMarker("12Allocate");
        ProfilerMarker m13 = new ProfilerMarker("13Allocate");
        ProfilerMarker m14 = new ProfilerMarker("14Allocate");
        ProfilerMarker m15 = new ProfilerMarker("15Allocate");
        ProfilerMarker m2 = new ProfilerMarker("2Allocate");
        ProfilerMarker m3 = new ProfilerMarker("3Allocate");
        ProfilerMarker m4 = new ProfilerMarker("4Allocate");



        Bounds bounds = new Bounds();
        bounds.min = new Vector3(0, 0, 0);
        bounds.max = new Vector3(chunkSize.x, chunkSize.y, chunkSize.z);


        for (int index = 0; index < chunks.Count; index++)
        {

            m.Begin();

            // TODO:  Create a generic Method to do this conversion and back
            NativeArray<ChunkMeshVertexData> _verts = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ChunkMeshVertexData>((void*)_vertsPtrs[index], _vertsCount[index] * 24, Allocator.None);
            AtomicSafetyHandle vertsAtomicHandle = AtomicSafetyHandle.Create();
            
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref _verts, vertsAtomicHandle);


            NativeArray<int> _tris = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)_trisPtrs[index], _trisCount[index], Allocator.None);
            AtomicSafetyHandle trisAtomicHandle = AtomicSafetyHandle.Create();
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref _tris, trisAtomicHandle);


            m.End();

            m1.Begin();


            m11.Begin();
            Mesh mesh = new Mesh();

            m11.End();
            m12.Begin();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m12.End();
            m13.Begin();
            mesh.SetVertexBufferParams(_vertsCount[index], _layout);
            m13.End();
            m14.Begin();
            mesh.SetVertexBufferData(_verts, 0, 0, _vertsCount[index]);
            m14.End();
            m15.Begin();
            mesh.SetIndices(_tris, 0, _trisCount[index], MeshTopology.Triangles, 0, false, 0);
            m15.End();

            m1.End();


            mesh.bounds = bounds;

            chunks[index].meshFilter.mesh = mesh;
            chunks[index].chunkObject.transform.position = new Vector3(chunkSize.x * chunks[index].pos.x, 0, chunkSize.z * chunks[index].pos.z);

            
            m4.Begin();
            AtomicSafetyHandle.Release(vertsAtomicHandle);
            AtomicSafetyHandle.Release(trisAtomicHandle);

            m4.End();
            
        }

    }




    private void generateMeshesForChunks(
        ref NativeArray<ulong> _blockDataPtrs,
        ref NativeArray<int> _blockDataCounts,
        ref NativeArray<int3> _chunkSize,
        ref NativeArray<ulong> _vertsPtrs,
        ref NativeArray<int> _vertsCount,
        ref NativeArray<ulong> _trisPtrs,
        ref NativeArray<int> _trisCount
        )
    {


        if (_blockDataPtrs.Length > 1)
        {


            JobHandle handle = new MultiMeshCreateJob()
            {
                blockDataPtrs = _blockDataPtrs,
                bockDataCounts = _blockDataCounts,
                chunkSize = _chunkSize,
                vertsPtrs = _vertsPtrs,
                vertCount = _vertsCount,
                trisPtrs = _trisPtrs,
                triIntCounts = _trisCount
            }.Schedule(_blockDataPtrs.Length, 1);

            handle.Complete();
        }
        else
        {
            // Premature Optimization, but don't bother scheduling if only running once
            new MultiMeshCreateJob()
            {
                blockDataPtrs = _blockDataPtrs,
                bockDataCounts = _blockDataCounts,
                chunkSize = _chunkSize,
                vertsPtrs = _vertsPtrs,
                vertCount = _vertsCount,
                trisPtrs = _trisPtrs,
                triIntCounts = _trisCount
            }.Run(1);

        }

    }


    public void Dispose()
    {

        foreach(var nativeArray in _vertsArray)
        {
            nativeArray.Dispose();
        }

        foreach (var nativeArray in _trisArray)
        {
            nativeArray.Dispose();
        }

    }











}
