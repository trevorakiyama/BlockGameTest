using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{


    




    public static VoxelFace getFrontFace()
    {

        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 0, 0);
        face.vertices[1] = new Vector3(0, 1, 0);
        face.vertices[2] = new Vector3(1, 0, 0);
        face.vertices[3] = new Vector3(1, 1, 0);

        face.uvs[0] = new Vector2(0, 0);
        face.uvs[1] = new Vector2(0, 1);
        face.uvs[2] = new Vector2(1, 0);
        face.uvs[3] = new Vector2(1, 1);

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


    public static VoxelFace getTopFace()
    {
        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 1, 0);
        face.vertices[1] = new Vector3(0, 1, 1);
        face.vertices[2] = new Vector3(1, 1, 0);
        face.vertices[3] = new Vector3(1, 1, 1);

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

        Vector3 normal = new Vector3(0, 1, 0);
        face.normals[0] = normal;
        face.normals[1] = normal;
        face.normals[2] = normal;
        face.normals[3] = normal;

        return face;
    }


    public static VoxelFace getBottomFace()
    {

        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 0, 0);
        face.vertices[1] = new Vector3(0, 0, 1);
        face.vertices[2] = new Vector3(1, 0, 0);
        face.vertices[3] = new Vector3(1, 0, 1);

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

    public static VoxelFace getBackFace()
    {
        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 0, 1);
        face.vertices[1] = new Vector3(0, 1, 1);
        face.vertices[2] = new Vector3(1, 0, 1);
        face.vertices[3] = new Vector3(1, 1, 1);

        face.uvs[0] = new Vector2(0, 0);
        face.uvs[1] = new Vector2(0, 1);
        face.uvs[2] = new Vector2(1, 0);
        face.uvs[3] = new Vector2(1, 1);

        Vector3 normal = new Vector3(0, 0, -1);
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

    public static VoxelFace getLeftFace()
    {
        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(0, 0, 0);
        face.vertices[1] = new Vector3(0, 1, 0);
        face.vertices[2] = new Vector3(0, 0, 1);
        face.vertices[3] = new Vector3(0, 1, 1);

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

        Vector3 normal = new Vector3(1, 0, 0);
        face.normals[0] = normal;
        face.normals[1] = normal;
        face.normals[2] = normal;
        face.normals[3] = normal;

        return face;
    }

    public static VoxelFace getRightFace()
    {
        VoxelFace face = new VoxelFace();

        face.vertices[0] = new Vector3(1, 0, 0);
        face.vertices[1] = new Vector3(1, 1, 0);
        face.vertices[2] = new Vector3(1, 0, 1);
        face.vertices[3] = new Vector3(1, 1, 1);

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
