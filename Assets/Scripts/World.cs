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


    //Chunk[,,] chunks = new Chunk[chunksWide, chunksHeight, chunksLong];

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
        



        //// Temporarily just initialize the chunks
        //for (int x = 0; x < chunksWide; x++)
        //{
        //    for (int y = 0; y < chunksHeight; y++)
        //    {
        //        for (int z = 0; z < chunksLong; z++)
        //        {

        //            Chunk newChunk = new Chunk(this, new Vector3Int(x, y, z));
        //            newChunk.initializeChunkData();

        //            //chunks[x,y,z] = newChunk;

        //            chunksd.Add(new Vector3Int(x, y, z), newChunk);
        //        }
        //    }
        //}
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





        

        //newChunk.initializeChunkData();
        //newChunk.renderChunk(new Vector3(chunkCoord.x * Chunk.chunkWidth, chunkCoord.y * Chunk.chunkHeight, chunkCoord.y * Chunk.chunkWidth));







        //chunksd[new Vector3Int(0, 0, 0)].renderChunk(new Vector3(0 * Chunk.chunkWidth, 0 * Chunk.chunkHeight, 0 * Chunk.chunkWidth));








        //for (int x = 0; x < chunksWide; x++)
        //{
        //    for (int y = 0; y < chunksHeight; y++)
        //    {
        //        for (int z = 0; z < chunksLong; z++)
        //        {
        //            //chunks[x, y, z].renderChunk(new Vector3(x * 16, y * 16, z * 16));
        //            if (chunksd.ContainsKey(new Vector3Int(x, y, z)))
        //                {
        //                chunksd[new Vector3Int(x, y, z)].renderChunk(new Vector3(x * Chunk.chunkWidth, y * Chunk.chunkHeight, z * Chunk.chunkWidth));
        //            }
        //        }
        //    }
        //}

    }

    
    private Vector3Int getChunkCoord(Vector3 pos)
    {
        Vector3Int posInt = Vector3Int.FloorToInt(pos);


        Vector3Int chunkCoord = new Vector3Int(posInt.x / Chunk.chunkWidth, posInt.y / Chunk.chunkHeight, posInt.z / Chunk.chunkWidth);

        return chunkCoord;
    }

    







    public Chunk getChunk(Vector3Int chunkAddress)
    {


        if (chunkAddress.x < 0 || chunkAddress.x >= chunksWide
            || chunkAddress.y < 0 || chunkAddress.x >= chunksHeight
            || chunkAddress.z < 0 || chunkAddress.x >= chunksLong )
        {
            return null;
        }


        //return chunks[chunkAddress.x, chunkAddress.y, chunkAddress.z];
        return chunksd[chunkAddress];
    }


    public static Block GetBlock(Vector3Int blockCoord)
    {

        // Calculate Chunk


        Vector3Int chunkCoord = Chunk.getChunkCoord(blockCoord);
        // if chunk does not yet exist, then return null;


        if (!instance.chunksd.ContainsKey(chunkCoord))
        {
            return null;
        }

        Chunk chunk = instance.chunksd[chunkCoord];


        Vector3Int blockOffset = Chunk.getChunkRelativeCoord(blockCoord);


        return chunk.chunkBlocks[blockOffset.x, blockOffset.y, blockOffset.z];
    }


    protected void OnDestroy()
    {

        foreach (Chunk chunk in chunksd.Values) {

            chunk.cleanup();

        }





    }


}
