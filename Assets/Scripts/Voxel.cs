using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Voxel
{


    enum FACE
    {
        TOP,
        BOTTOM,
        NORTH,
        SOUTH,
        WEST,
        EAST
    }



    static Vector2[] uvs = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1,1)
        };

    static Vector3[] normals = new Vector3[]
    {
        new Vector3(0,1,0), // Top
        new Vector3(0,-1,0),
        new Vector3(0,0,0),
        new Vector3(0,0,0),
        new Vector3(0,0,0),
        new Vector3(0,0,0),


    };


    static Vector3[] vertices = new Vector3[]
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(0,1,0),
        new Vector3(1,1,0),
        new Vector3(0,0,1),
        new Vector3(1,0,1),
        new Vector3(0,1,1),
        new Vector3(1,1,1)
    };






    
    /**
     * Front
     * Verts = 0,1,2,3
     * Tris = 9,1,2,2,1,3
     * norm =  0,0,-1
     * 
     * top
     * verts = 2,6,3,7
     * TriVerts = 0,1,2,2,1,3
     * norm = 0,1,0
     * 
     * 
     * Bottom
     * verts = 0,4,1,5
     * TriVerts = 0,2,1,1,2,3
     * 

    

    */


    public static VoxelFace getFrontFace(Vector3 delta)
    {

        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 0, 0) + delta;
        face.vertices[1] = new Vector3(0, 1, 0) + delta;
        face.vertices[2] = new Vector3(1, 0, 0) + delta;
        face.vertices[3] = new Vector3(1, 1, 0) + delta;

        face.uvs[0] = uvs[0];
        face.uvs[1] = uvs[1];
        face.uvs[2] = uvs[2];
        face.uvs[3] = uvs[3];

        Vector3 normal = new Vector3(0, 0, -1);
        face.normals[0] = normal;
        face.normals[1] = normal;
        face.normals[2] = normal;
        face.normals[3] = normal;

        face.triangles[0] = 0;
        face.triangles[1] = 1;
        face.triangles[2] = 2;
        face.triangles[3] = 2;
        face.triangles[4] = 1;
        face.triangles[5] = 3;


        return face;
    }


    public static VoxelFace getTopFace(Vector3 delta)
    {
        //Profiler.BeginSample("Top Face Method2");

        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 1, 0) + delta;
        face.vertices[1] = new Vector3(0, 1, 1) + delta;
        face.vertices[2] = new Vector3(1, 1, 0) + delta;
        face.vertices[3] = new Vector3(1, 1, 1) + delta;

        face.vertices[0].x = vertices[2].x + delta.x;
        face.vertices[0].y = vertices[2].y + delta.y;
        face.vertices[0].z = vertices[2].z + delta.z;

        face.vertices[1].x = vertices[6].x + delta.x;
        face.vertices[1].y = vertices[6].y + delta.y;
        face.vertices[1].z = vertices[6].z + delta.z;

        face.vertices[2].x = vertices[3].x + delta.x;
        face.vertices[2].y = vertices[3].y + delta.y;
        face.vertices[2].z = vertices[3].z + delta.z;

        face.vertices[3].x = vertices[7].x + delta.x;
        face.vertices[3].y = vertices[7].y + delta.y;
        face.vertices[3].z = vertices[7].z + delta.z;


        //face.vertices[0] = vertices[2] + delta;
        //face.vertices[1] = vertices[6] + delta;
        //face.vertices[2] = vertices[3] + delta;
        //face.vertices[3] = vertices[7] + delta;


        face.triangles[0] = 0;
        face.triangles[1] = 1;
        face.triangles[2] = 2;
        face.triangles[3] = 2;
        face.triangles[4] = 1;
        face.triangles[5] = 3;

        face.uvs[0] = uvs[0];
        face.uvs[1] = uvs[1];
        face.uvs[2] = uvs[2];
        face.uvs[3] = uvs[3];

        //Vector3 normal = new Vector3(0, 1, 0);
        face.normals[0] = normals[0];
        face.normals[1] = normals[0];
        face.normals[2] = normals[0];
        face.normals[3] = normals[0];

        //Profiler.EndSample();

        return face;
    }





    public static VoxelFace getTopFace2(Vector3 delta)
    {
        //Profiler.BeginSample("Top Face Method2");

        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 1, 0) + delta;
        face.vertices[1] = new Vector3(0, 1, 1) + delta;
        face.vertices[2] = new Vector3(1, 1, 0) + delta;
        face.vertices[3] = new Vector3(1, 1, 1) + delta;

        face.vertices[0].x = vertices[2].x + delta.x;
        face.vertices[0].y = vertices[2].y + delta.y;
        face.vertices[0].z = vertices[2].z + delta.z;

        face.vertices[1].x = vertices[6].x + delta.x;
        face.vertices[1].y = vertices[6].y + delta.y;
        face.vertices[1].z = vertices[6].z + delta.z;

        face.vertices[2].x = vertices[3].x + delta.x;
        face.vertices[2].y = vertices[3].y + delta.y;
        face.vertices[2].z = vertices[3].z + delta.z;

        face.vertices[3].x = vertices[7].x + delta.x;
        face.vertices[3].y = vertices[7].y + delta.y;
        face.vertices[3].z = vertices[7].z + delta.z;


        //face.vertices[0] = vertices[2] + delta;
        //face.vertices[1] = vertices[6] + delta;
        //face.vertices[2] = vertices[3] + delta;
        //face.vertices[3] = vertices[7] + delta;


        face.triangles[0] = 0;
        face.triangles[1] = 1;
        face.triangles[2] = 2;
        face.triangles[3] = 2;
        face.triangles[4] = 1;
        face.triangles[5] = 3;

        face.uvs[0] = uvs[0];
        face.uvs[1] = uvs[1];
        face.uvs[2] = uvs[2];
        face.uvs[3] = uvs[3];

        //Vector3 normal = new Vector3(0, 1, 0);
        face.normals[0] = normals[0];
        face.normals[1] = normals[0];
        face.normals[2] = normals[0];
        face.normals[3] = normals[0];

        //Profiler.EndSample();

        return face;
    }










    public static VoxelFace getBottomFace(Vector3 delta)
    {

        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 0, 0) + delta;
        face.vertices[1] = new Vector3(0, 0, 1) + delta;
        face.vertices[2] = new Vector3(1, 0, 0) + delta;
        face.vertices[3] = new Vector3(1, 0, 1) + delta;

        face.triangles[0] = 0;
        face.triangles[1] = 2;
        face.triangles[2] = 1;
        face.triangles[3] = 1;
        face.triangles[4] = 2;
        face.triangles[5] = 3;

        face.uvs[0] = new Vector2(0, 0);
        face.uvs[1] = new Vector2(0, 1);
        face.uvs[2] = new Vector2(1, 0);
        face.uvs[3] = new Vector2(1, 1);

        Vector3 normal = new Vector3(0, -1, 0);
        face.normals[0] = normal;
        face.normals[1] = normal;
        face.normals[2] = normal;
        face.normals[3] = normal;

        return face;


    }

    public static VoxelFace getBackFace(Vector3 delta)
    {
        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 0, 1) + delta;
        face.vertices[1] = new Vector3(0, 1, 1) + delta;
        face.vertices[2] = new Vector3(1, 0, 1) + delta;
        face.vertices[3] = new Vector3(1, 1, 1) + delta;

        face.uvs[0] = new Vector2(0, 0);
        face.uvs[1] = new Vector2(0, 1);
        face.uvs[2] = new Vector2(1, 0);
        face.uvs[3] = new Vector2(1, 1);

        Vector3 normal = new Vector3(0, 0, 1);
        face.normals[0] = normal;
        face.normals[1] = normal;
        face.normals[2] = normal;
        face.normals[3] = normal;

        face.triangles[0] = 0;
        face.triangles[1] = 2;
        face.triangles[2] = 1;
        face.triangles[3] = 1;
        face.triangles[4] = 2;
        face.triangles[5] = 3;

        return face;
    }

    public static VoxelFace getLeftFace(Vector3 delta)
    {
        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 0, 0) + delta;
        face.vertices[1] = new Vector3(0, 1, 0) + delta;
        face.vertices[2] = new Vector3(0, 0, 1) + delta;
        face.vertices[3] = new Vector3(0, 1, 1) + delta;

        face.triangles[0] = 0;
        face.triangles[1] = 2;
        face.triangles[2] = 1;
        face.triangles[3] = 1;
        face.triangles[4] = 2;
        face.triangles[5] = 3;

        face.uvs[0] = new Vector2(1, 0);
        face.uvs[1] = new Vector2(1, 1);
        face.uvs[2] = new Vector2(0, 0);
        face.uvs[3] = new Vector2(0, 1);

        Vector3 normal = new Vector3(-1, 0, 0);
        face.normals[0] = normal;
        face.normals[1] = normal;
        face.normals[2] = normal;
        face.normals[3] = normal;

        return face;
    }

    public static VoxelFace getRightFace(Vector3 delta)
    {
        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(1, 0, 0) + delta;
        face.vertices[1] = new Vector3(1, 1, 0) + delta;
        face.vertices[2] = new Vector3(1, 0, 1) + delta;
        face.vertices[3] = new Vector3(1, 1, 1) + delta;

        face.triangles[0] = 0;
        face.triangles[1] = 1;
        face.triangles[2] = 2;
        face.triangles[3] = 2;
        face.triangles[4] = 1;
        face.triangles[5] = 3;

        face.uvs[0] = new Vector2(0, 0);
        face.uvs[1] = new Vector2(0, 1);
        face.uvs[2] = new Vector2(1, 0);
        face.uvs[3] = new Vector2(1, 1);

        Vector3 normal = new Vector3(1, 0, 0);
        face.normals[0] = normal;
        face.normals[1] = normal;
        face.normals[2] = normal;
        face.normals[3] = normal;

        return face;
    }


}
