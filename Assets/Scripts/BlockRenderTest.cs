
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public class BlockRenderTest : MonoBehaviour
{

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public static int chunkHeight = 256;
    public static int chunkWidth = 16;


    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        UpdateMesh();

        stopwatch.Stop();

        Debug.LogFormat("Mesh Full Create time ={0}", stopwatch.Elapsed);

    }



    void generateVoxels()
    {
        // TODO:  Will require seeds for more interesting terrain
    }


    void UpdateMesh()
    {

        // Walk through all the cubes and add to mesh

        List<VoxelFace> faces = new List<VoxelFace>();




        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {

                    // for now assume it's a solid testblock


                    // check all adjacent blocks for solidness
                    // if solid then don't add the face
                    // if not solid then add the face


                    //// Top
                    //if (y == chunkHeight - 1)
                    //{
                    //    Profiler.BeginSample("Adding Top Face");
                    //    faces.Add(Voxel.getTopFace(new Vector3(x, y, z)));
                    //    Profiler.EndSample();
                    //}

                    //// Bottom
                    //if (y == 0)
                    //{
                    //    faces.Add(Voxel.getBottomFace(new Vector3(x, y, z)));
                    //}

                    //// Right
                    //if (x == chunkWidth - 1)
                    //{
                    //    faces.Add(Voxel.getRightFace(new Vector3(x, y, z)));
                    //}

                    //// Left
                    //if (x == 0)
                    //{
                    //    faces.Add(Voxel.getLeftFace(new Vector3(x, y, z)));
                    //}

                    //// Back
                    //if (z == chunkWidth - 1)
                    //{
                    //    faces.Add(Voxel.getBackFace(new Vector3(x, y, z)));
                    //}

                    // Front
                    if (z == 0)
                    {
                        faces.Add(Voxel.getFrontFace(new Vector3(x, y, z)));
                    }
                }
            }

        }



        // Render faces

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        buildMesh(faces);

        stopwatch.Stop();

        Debug.LogFormat("Mesh Build time ={0}", stopwatch.Elapsed);
    }


    void buildMesh(List<VoxelFace> faces)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        int faceCount = 0;
        foreach (VoxelFace face in faces)
        {

            // copy the Vertex information
            for (int i = 0; i < 4; i++)
            {
                vertices.Add(face.vertices[i]);
                uvs.Add(face.uvs[i]);
                normals.Add(face.normals[i]);
            }

            for (int i = 0; i < 6; i++)
            {
                triangles.Add(face.triangles[i] + faceCount * 4);
            }

            faceCount++;
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.normals = normals.ToArray();

        mesh.normals = normals.ToArray();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //mesh.RecalculateNormals();

        stopwatch.Stop();

        Debug.LogFormat("Normal Recalc time ={0}", stopwatch.Elapsed);

        meshFilter.mesh = mesh;
    }




    // Update is called once per frame
    void Update()
    {
        UpdateMesh();
    }
}
