using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

public class ChunkMeshes
{

    NativeArray<ChunkMeshVertexData> _verts;
    NativeArray<int> _tris;



    public ChunkMeshes(int3 chunkSize)
    {
        _verts = new NativeArray<ChunkMeshVertexData>(chunkSize.x * chunkSize.y * chunkSize.z * 24, Allocator.Persistent);
        _tris = new NativeArray<int>(chunkSize.x * chunkSize.y * chunkSize.z * 36, Allocator.Persistent);

    }




    // TODO:  This unsafe stuff should be moved into it's own Class

    unsafe public void generateMeshesForChunks(int3 chunkSize, int3 chunkCoord, NativeArray<ChunkBlockData> blockData, MeshFilter meshFilter, Transform transform)
    {

        ProfilerMarker marker = new ProfilerMarker("GenerateMeshes");
        ProfilerMarker m1 = new ProfilerMarker("g1");
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



        // TEMPORARY: TODO Convert the single data into a multi data form


        m1.Begin();

        // Inputs
        NativeArray<ulong> _blockDataPtrs = new NativeArray<ulong>(1, Allocator.TempJob);
        _blockDataPtrs[0] = (ulong)blockData.GetUnsafePtr();

        NativeArray<int> _blockDataCounts = new NativeArray<int>(1, Allocator.TempJob);
        _blockDataCounts[0] = blockData.Length;

        NativeArray<int3> _chunkSize = new NativeArray<int3>(1, Allocator.TempJob);
        _chunkSize[0] = chunkSize;



        // Outputs

        NativeArray<ulong> _vertsPtrs = new NativeArray<ulong>(1, Allocator.TempJob);
        NativeArray<int> _vertsCount = new NativeArray<int>(1, Allocator.TempJob);
        NativeArray<ulong> _trisPtrs = new NativeArray<ulong>(1, Allocator.TempJob);
        NativeArray<int> _trisCount = new NativeArray<int>(1, Allocator.TempJob);

        m1.End();


        _vertsPtrs[0] = (ulong)_verts.GetUnsafePtr();
        _trisPtrs[0] = (ulong)_tris.GetUnsafePtr();

        m2.Begin();

        // Operate
        generateMeshesForChunks(ref _blockDataPtrs, ref _blockDataCounts, ref _chunkSize,
            ref _vertsPtrs, ref _vertsCount, ref _trisPtrs, ref _trisCount);

        m2.End();
        m31.Begin();


        // Index is a temp way to do one chunk
        int index = 0;


        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        m31.End();

        m3.Begin();


        // TODO:  This could probably be put somewhere else
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
        };


        mesh.SetVertexBufferParams(_vertsCount[index], layout);
        mesh.SetVertexBufferData(_verts, 0, 0, _vertsCount[index]);

        m3.End();
        m4.Begin();


        NativeSlice<int> slice = _tris.Slice<int>(0, _trisCount[index]);
        mesh.triangles = slice.ToArray();

        m4.End();

        Bounds bounds = new Bounds();
        bounds.min = new Vector3(0, 0, 0);
        bounds.max = new Vector3(chunkSize.x, chunkSize.y, chunkSize.z);

        m5.Begin();

        mesh.bounds = bounds;


        meshFilter.mesh = mesh;
        transform.position = new Vector3(chunkSize.x * chunkCoord.x, 0, chunkSize.z * chunkCoord.z);

        _blockDataPtrs.Dispose();
        _blockDataCounts.Dispose();
        _chunkSize.Dispose();

        _vertsPtrs.Dispose();
        _vertsCount.Dispose();
        _trisPtrs.Dispose();
        _trisCount.Dispose();

        m5.End();


        marker.End();

    }



    // TODO:  This should be moved out of here
    public void generateMeshesForChunks(
        ref NativeArray<ulong> _blockDataPtrs,
        ref NativeArray<int> _blockDataCounts,
        ref NativeArray<int3> _chunkSize,
        ref NativeArray<ulong> _vertsPtrs,
        ref NativeArray<int> _vertCount,
        ref NativeArray<ulong> _trisPtrs,
        ref NativeArray<int> _trisCount
        )
    {


        JobHandle handle = new MultiMeshCreateJob()
        {
            blockDataPtrs = _blockDataPtrs,
            bockDataCounts = _blockDataCounts,
            chunkSize = _chunkSize,
            vertsPtrs = _vertsPtrs,
            vertCount = _vertCount,
            trisPtrs = _trisPtrs,
            triIntCounts = _trisCount
        }.Schedule(_blockDataPtrs.Length, 1);

        handle.Complete();

    }


    public void Dispose()
    {
        _verts.Dispose();
        _tris.Dispose();

    }











}
