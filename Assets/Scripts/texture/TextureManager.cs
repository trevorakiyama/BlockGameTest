using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureManager : MonoBehaviour
{

    public Material material;

    public static TextureManager instance;



    public void Start()
    {
        instance = this;
    }


    public void Update()
    {
        
    }






    // get map coords for 


    // texture is 64x64
    // 4 faces x 4 faces

    private static int textureSize = 64;
    private static int tilesx = 4;
    private static float dx = (float)textureSize / tilesx;





    public static (Vector2, Vector2) GetUVCoords(int index)
    {


        // TODO develop a unit test for this



        // figure out the x and y value;


        int y = index / tilesx;
        int x = index % tilesx;



        float uvx = x;
        float uvy = y;

        return (new Vector2(uvx, uvy), new Vector2(uvx +1, uvy + 1));
    }


    public static Material GetMaterial()
    {
        return instance.material;
    }


    




}
