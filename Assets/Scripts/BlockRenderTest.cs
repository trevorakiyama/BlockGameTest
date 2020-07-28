using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public class BlockRenderTest : MonoBehaviour
{

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;


    // Start is called before the first frame update
    void Start()
    {

        // Create a Vertex array

        //List<Vector3> vertices = new List<Vector3>();
        //List<int> triangles = new List<int>();
        //List<Vector2> uvs = new List<Vector2>();

        //vertices.Add(new Vector3(0, 0, 0));
        //vertices.Add(new Vector3(0, 1, 0));
        //vertices.Add(new Vector3(1, 0, 0));
        //vertices.Add(new Vector3(1, 1, 0));



        //triangles.Add(0);
        //triangles.Add(1);
        //triangles.Add(2);
        //triangles.Add(2);
        //triangles.Add(1);
        //triangles.Add(3);


        //uvs.Add(new Vector2(0, 0));
        //uvs.Add(new Vector2(0, 1));
        //uvs.Add(new Vector2(1, 0));
        //uvs.Add(new Vector2(1, 1));


        VoxelFace frontFaceData = Voxel.getFrontFace();
        VoxelFace topFaceData = Voxel.getTopFace();


        List<VoxelFace> faces = new List<VoxelFace>();


        faces.Add(Voxel.getFrontFace());
        faces.Add(Voxel.getTopFace());
        faces.Add(Voxel.getLeftFace());
        faces.Add(Voxel.getRightFace());
        faces.Add(Voxel.getBottomFace());
        faces.Add(Voxel.getBackFace());




        // TODO:  might want to investigate turning this into arrays rather than lists
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        int faceCount = 0;
        foreach( VoxelFace face in faces){

            // copy the Vertex information
            for(int i = 0; i < 4; i++)
            {
                vertices.Add(face.vertices[i]);
                uvs.Add(face.uvs[i]);
                normals.Add(face.normals[i]);
            }

            for (int i = 0; i<6; i++)
            {
                triangles.Add(face.triangles[i] + faceCount * 4);
            }

            faceCount++;
        }





        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        //mesh.normals = normals.ToArray();


        mesh.RecalculateNormals();



        List<Vector3> calcnormals = new List<Vector3>();

        mesh.GetNormals(calcnormals);

        foreach(Vector3 normal in calcnormals)
        {
            Debug.Log(normal.x + " " + normal.y + " " + normal.z);




        }
        


        Debug.Log(calcnormals[0].x + " " + calcnormals[0].y + " " + calcnormals[0].z);


        meshFilter.mesh = mesh;

        
    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
