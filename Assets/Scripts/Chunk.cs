using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.Rendering;

/// <summary>
/// Class <c>Chunk</c> represents a chunk of block voxels for managmeent and rendering.
/// </summary>
public class Chunk
{
    public GameObject chunkObject;

    internal MeshRenderer meshRenderer;

    internal MeshFilter meshFilter;

    internal World world;

    internal int3 pos;

    // A chunk will contain:
    // A mesh object for manipulation and rendering
    // an 3d array representing all the block in the chunk
    public const int chunkWidth = 16;

    public const int chunkHeight = 16;

    //public NativeArray<BlockData> blockData = new NativeArray<BlockData>(chunkWidth * chunkWidth * chunkHeight, Allocator.Persistent);

    internal Boolean dataInitialized = false;

    internal Boolean recalculateMesh = true;

    public ChunkManager chunkManager;


    /** States
     * Not Initialized
     * Initializing
     * Initialized
     * Mesh needed
     * Mesh generating
     * Mesh Generated
    */

    Boolean initialized = false;
    Boolean initialization = false;
    Boolean meshCreated = false;
    Boolean meshGenerating = false;

    JobHandle initiHandle;
    JobHandle meshCreateHandle;



    /// <summary>
    /// Initializes a new instance of the <see cref="Chunk"/> class.
    /// </summary>
    /// <param name="world">The world<see cref="World"/>.</param>
    /// <param name="pos">The pos<see cref="Vector3Int"/>.</param>
    public Chunk(World world, int3 pos, ChunkManager chunkManager)
    {

        this.chunkManager = chunkManager;

        this.world = world;
        this.pos = pos;
        chunkObject = new GameObject();
        chunkObject.name = "This Named Chunk" + pos;
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        //meshRenderer.material = world.textureMaterials;
        meshRenderer.material = world.textureMaterials;

        recalculateMesh = true;

        


    }
    
}
