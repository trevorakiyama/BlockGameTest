using System.Collections.Generic;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;
using Unity.Mathematics;

/// <summary>
/// Defines the <see cref="World" />.
/// </summary>
public class World : MonoBehaviour
{
    public static readonly int chunksWide = 1;

    public static readonly int chunksHeight = 1;

    public static readonly int chunksLong = 1;

    internal Dictionary<Vector3Int, Chunk> chunksd = new Dictionary<Vector3Int, Chunk>();

    public Material textureMaterials;

    private static World instance;

    internal Vector3 playerPosition;


    public ChunkManager chunkManager;



    // Start is called before the first frame update
    /// <summary>
    /// The Start.
    /// </summary>
    public void Start()
    {

        instance = this;

        chunkManager = new ChunkManager(this);
    }

    internal ProfilerMarker marker1 = new ProfilerMarker("Update 1");

    internal ProfilerMarker marker2 = new ProfilerMarker("Update 2");

    // Update is called once per frame
    /// <summary>
    /// The Update.
    /// </summary>
    internal void Update()
    {

        

        // TODO:  Make a real call through
        chunkManager.checkUpdates();




        GameObject player = GameObject.Find("Camera2");

        playerPosition = player.transform.position;

        Vector3Int chunkCoord = getChunkCoord(playerPosition);

        marker1.Begin();

        chunkManager.ProcessChunksMeshes(new int3(chunkCoord.x, chunkCoord.y, chunkCoord.z));

        marker1.End();
    }

    /// <summary>
    /// The getChunkCoord.
    /// </summary>
    /// <param name="pos">The pos<see cref="Vector3"/>.</param>
    /// <returns>The <see cref="Vector3Int"/>.</returns>
    private Vector3Int getChunkCoord(Vector3 pos)
    {


        Vector3Int posInt = Vector3Int.FloorToInt(pos);
        if (posInt.x < 0)
        {
            posInt.x -= Chunk.chunkWidth;
        }

        //if (posInt.y < 0)
        //{
        //    posInt.y -= Chunk.chunkHeight;
        //}

        // for now just use y = 0 always;
        posInt.y = 0;



        if (posInt.z < 0)
        {
            posInt.z -= Chunk.chunkWidth;
        }


        Vector3Int chunkCoord = new Vector3Int(posInt.x / Chunk.chunkWidth, posInt.y / Chunk.chunkHeight, posInt.z / Chunk.chunkWidth);

        return chunkCoord;
    }

    /// <summary>
    /// The OnDestroy.
    /// </summary>
    protected void OnDestroy()
    {

        foreach (Chunk chunk in chunksd.Values)
        {

            Destroy(chunk.
                chunkObject);

        }

        chunkManager.Dispose();
    }

    public void DestroyObject(GameObject obj)
    {

        //DestroyObject(obj);
        
        obj.SetActive(false);
        
        Destroy(obj);

    }


}
