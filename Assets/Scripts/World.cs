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

    // TODO: Consider making this a NativeHashMap instead (Should Chunk contain somethign more?
    internal Dictionary<Vector3Int, Chunk> chunksd = new Dictionary<Vector3Int, Chunk>();

    // TODO: Consider moving this to a Texture Manager
    public Material textureMaterials;

    // TODO: Is this even needed?
    private static World instance;


    // TODO: Perhaps Player should be an entity?
    internal float3 playerPosition;


    public ChunkManager chunkManager;



    public static int renderDistance = 32;
    public static int retentionDistance = 48;


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

        // Each frame of the Update


        // Temporary for now as Camera will later follow the player not vice versa
        GameObject player = GameObject.Find("Camera2");
        playerPosition = player.transform.position;

        int3 playerChunkCoord = getChunkCoord(playerPosition);


        // Kick off Loading and Initializing Any new Chunks that need to be loaded

        // InitializeChunks  (Base Position, radius to check)





        // Update Player, and gameworld state and entity AI on active chunks



        // Update entities states




        // Generate Meshes
        // GenerateMeshes for outstanding initialized chunks 
        // Generate Meshes (Base Position, radius to check)



        // Save and Unload Chunks out of retention range
        // Unload Meshes (Base Position, Radius to check);



       

       

        marker1.Begin();

        chunkManager.ProcessChunksMeshes(new int3(playerChunkCoord.x, playerChunkCoord.y, playerChunkCoord.z));

        


        marker1.End();
    }






    // TODO: This needs to move somewhere else
    /// <summary>
    /// The getChunkCoord.
    /// </summary>
    /// <param name="pos">The pos<see cref="Vector3"/>.</param>
    /// <returns>The <see cref="Vector3Int"/>.</returns>
    private int3 getChunkCoord(float3 pos)
    {

        int3 chunkSize = ChunkManager.chunkSize;

        Vector3Int posInt = Vector3Int.FloorToInt(pos);
        if (posInt.x < 0)
        {
            posInt.x -= chunkSize.x;
        }

        //if (posInt.y < 0)
        //{
        //    posInt.y -= Chunk.chunkHeight;
        //}

        // for now just use y = 0 always;
        posInt.y = 0;



        if (posInt.z < 0)
        {
            posInt.z -= chunkSize.y;
        }


        int3 chunkCoord = new int3(posInt.x / chunkSize.x, posInt.y / chunkSize.y, posInt.z / chunkSize.z);

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
