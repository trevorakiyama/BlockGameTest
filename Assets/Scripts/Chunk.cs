using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.Rendering;

// TODO:  Convert this to an entity for better ECS Mgmt
public struct Chunk
{
    public GameObject chunkObject;
    internal MeshFilter meshFilter;
    public Boolean dataInitialized;
    public Boolean recalculateMesh;

    internal int3 pos;

    
    public Chunk(Material material, int3 pos, ChunkManager chunkManager)
    {

        this.pos = pos;
        chunkObject = new GameObject();
        chunkObject.name = "Chunk" + pos;
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        dataInitialized = false;
        recalculateMesh = true;
    }
}
