using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEngine;

public class World : MonoBehaviour
{


    public static readonly int chunksWide = 1;
    public static readonly int chunksHeight = 1;
    public static readonly int chunksLong = 1;


    Dictionary<Vector3Int, Chunk> chunksd = new Dictionary<Vector3Int, Chunk>();



    public Material textureMaterials;

    private static World instance;

    Vector3 playerPosition;




    // Start is called before the first frame update
    public void Start()
    {
       
        instance = this;


        Chunk newChunk = new Chunk(this, new Vector3Int(0, 0, 0));
        newChunk.initializeChunkData();


        chunksd.Add(new Vector3Int(0,0,0), newChunk);

    }

    ProfilerMarker marker1 = new ProfilerMarker("Update 1");
    ProfilerMarker marker2 = new ProfilerMarker("Update 2");


    // Update is called once per frame
    void Update()
    {

        GameObject player = GameObject.Find("Camera2");

        playerPosition = player.transform.position;



        // render the chunk that the player is on

        // get coordinates of chunk that player is in
        // initialize Chunk so it can render



        Vector3Int chunkCoord = getChunkCoord(playerPosition) ;




        for (int x = chunkCoord.x - 1; x < chunkCoord.x  ; x++)
        {
            for (int y = 0; y <= 0; y++)
            {
                for (int z = chunkCoord.z - 1; z < chunkCoord.z ; z++)
                {

                    Vector3Int currChunk = new Vector3Int(x, y, z);

                    Chunk foundChunk;

                    bool found = chunksd.TryGetValue(currChunk, out foundChunk);


                    if (!found)
                    {

                        Debug.LogFormat("Chunk Pos {0} {1}", chunkCoord.ToString(), playerPosition);
                        marker1.Begin();
                        foundChunk = new Chunk(this, currChunk);
                        marker1.End();

                        //foundChunk.initializeChunkData();
                        chunksd.Add(currChunk, foundChunk);

                        
                        
                    }

                    marker2.Begin();
                    foundChunk.renderChunk(new Vector3(currChunk.x * Chunk.chunkWidth, currChunk.y * Chunk.chunkHeight, currChunk.z * Chunk.chunkWidth));
                    marker2.End();


                }
            }


        }

    }

    
    private Vector3Int getChunkCoord(Vector3 pos)
    {
        Vector3Int posInt = Vector3Int.FloorToInt(pos);


        Vector3Int chunkCoord = new Vector3Int(posInt.x / Chunk.chunkWidth, posInt.y / Chunk.chunkHeight, posInt.z / Chunk.chunkWidth);

        return chunkCoord;
    }



    protected void OnDestroy()
    {

        foreach (Chunk chunk in chunksd.Values) {

            chunk.cleanup();

        }





    }


}
