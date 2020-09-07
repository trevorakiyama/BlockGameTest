
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

    static ProfilerMarker marker = new ProfilerMarker("NEIGBOR");

    //public int addVoxelMeshData(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals, int vertexCount)
    //{



    //    Boolean[] renderFaces = new bool[6];
    //    Boolean foundFace = false;
    //    List<VoxelFace> faces = new List<VoxelFace>();


    //    for (int i = 0; i < 6; i++)
    //    {
    //        marker.Begin();
    //        Block neighbor = getNeighborBlock(faceDirVec[i]);
    //        marker.End();

    //        if (neighbor != null && neighbor.isSolid)
    //        {
    //            renderFaces[i] = false;
    //            continue;
    //        }
    //        foundFace = true;
    //        renderFaces[i] = true;
    //    }

    //    if (foundFace == false)
    //    {
    //        return 0;
    //    }


    //    return Voxel.addVoxelMeshData(renderFaces, Chunk.getChunkRelativeCoord(myPosWorld), vertices, triangles, uvs, normals, vertexCount);
    //}







    //public VoxelMeshData GetVoxelMeshData()
    //{
    //    // Perhaps this should go into a separate Block Renderer 

    //    // create a new VoxelData Object for this bloxk
    //    // This Voxel Data will be sent to create a Mesh 

    //    // Note this is probably too exepensive but clearer for right now
    //    // Maybe even get some of this from Lua?

    //    if (!isVisible)
    //    {
    //        return null;
    //    }

    //    // determine the faces that need to be added and then add them



    //    Boolean[] renderFaces = new bool[6];
    //    Boolean foundFace = false;
    //    List<VoxelFace> faces = new List<VoxelFace>();


    //    for (int i = 0; i < 6; i++)
    //    {
    //        Block neighbor = getNeighborBlock(faceDirVec[i]);

    //        if (neighbor != null  && neighbor.isSolid)
    //        {
    //            renderFaces[i] = false;
    //            continue;
    //        }
    //        foundFace = true;
    //        renderFaces[i] = true;
    //    }

    //    if (foundFace == false)
    //    {
    //        return null;
    //    }



    //    return Voxel.getVoxelMeshData(renderFaces, Chunk.getChunkRelativeCoord(myPosWorld));
    //}





    //private Block getNeighborBlock(Vector3Int neighborRelative)
    //{
    //    int x = myPosChunk.x + neighborRelative.x;
    //    int y = myPosChunk.y + neighborRelative.y;
    //    int z = myPosChunk.z + neighborRelative.z;

    //    if (x < 0 || x >= Chunk.chunkWidth
    //        || y < 0 || y >= Chunk.chunkHeight
    //        || z < 0 || z >= Chunk.chunkWidth)
    //    {
    //        // TODO:  Lookup the neighbor chunks per mesh render because they can be very expensive when called a lot
    //        return World.GetBlock(myPosWorld + neighborRelative);
    //    } 
    //    else
    //    {
    //        //return myChunk.chunkBlocks[x, y, z];
    //        throw new NotImplementedException("NOT IMPEMENTED YET");
    //    }

    //}


}
