using System.Collections.Generic;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;

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

    // Start is called before the first frame update
    /// <summary>
    /// The Start.
    /// </summary>
    public void Start()
    {

        instance = this;


        Chunk newChunk = new Chunk(this, new Vector3Int(0, 0, 0));
        newChunk.initializeChunkData();


        chunksd.Add(new Vector3Int(0, 0, 0), newChunk);
    }

    internal ProfilerMarker marker1 = new ProfilerMarker("Update 1");

    internal ProfilerMarker marker2 = new ProfilerMarker("Update 2");

    // Update is called once per frame
    /// <summary>
    /// The Update.
    /// </summary>
    internal void Update()
    {



        GameObject player = GameObject.Find("Camera2");

        playerPosition = player.transform.position;


        // render the chunk that the player is on

        // get coordinates of chunk that player is in
        // initialize Chunk so it can render



        Vector3Int chunkCoord = getChunkCoord(playerPosition);



        // Make the Circle based on the player position, not just the chunk
        
        NativeList<Vector3Int> nearChunks = new NativeList<Vector3Int>(0, Allocator.TempJob);

        CircularSearchJob job = new CircularSearchJob();
        job.orderedCoords = nearChunks;
        job.origin = chunkCoord;
        job.maxDist = 64;
        JobHandle handle = job.Schedule();

        handle.Complete();


        long timestamp = System.Diagnostics.Stopwatch.GetTimestamp();


            for (int i = 0; i < job.orderedCoords.Length; i++)
        {

            // check if the chunk is already in Chunkd, if it is not, then generate it, otherwise skip it

            Vector3Int currChunk = job.orderedCoords[i];

            Chunk foundChunk;
            bool found = chunksd.TryGetValue(currChunk, out foundChunk);

            if (!found)
            {

                Debug.LogFormat("Chunk NEW Pos {0} {1}", chunkCoord.ToString(), playerPosition);
                marker1.Begin();
                foundChunk = new Chunk(this, currChunk);
                marker1.End();

                //foundChunk.initializeChunkData();
                chunksd.Add(currChunk, foundChunk);





            }


            foundChunk.renderChunk(new Vector3(currChunk.x * Chunk.chunkWidth, currChunk.y * Chunk.chunkHeight, currChunk.z * Chunk.chunkWidth));

            if (System.Diagnostics.Stopwatch.GetTimestamp() - timestamp > 100000)
            {
                break;
            }



        }



            nearChunks.Dispose();

        

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

        if (posInt.y < 0)
        {
            posInt.y -= Chunk.chunkHeight;
        }

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

            chunk.cleanup();

        }
    }
}
