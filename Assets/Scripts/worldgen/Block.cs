
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine;

public class Block
{

    public enum faceDir {
        TOP =0 ,
        BOTTOM =1 ,
        NORTH = 2,
        SOUTH = 3 ,
        EAST = 4 ,
        WEST = 5
    }

    public Vector3Int[] faceDirVec = new Vector3Int[]
    {
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0),
        new Vector3Int(0,0,1),
        new Vector3Int(0,0,-1),
        new Vector3Int(1,0,0),
        new Vector3Int(-1,0,0)

    };



    public string name;
    public Boolean isSolid;
    public Boolean isVisible;
    public Vector3Int myPosWorld;
    public Vector3Int myPosChunk;
    public Chunk myChunk;
    //public int[] faceTextureIndices = new int[6];


   

    public Block(Chunk myChunk, Vector3Int myPosWorld, Vector3Int myPosChunk, Boolean isSolid, Boolean isVisible)
    {
        this.myChunk = myChunk;
        this.isSolid = isSolid;
        this.isVisible = isVisible;
        this.myPosWorld = myPosWorld;
        this.myPosChunk = myPosChunk;
    }

}
