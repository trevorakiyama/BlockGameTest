using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks
{



    public Block[] blocks = new Block[16];



    public Blocks()
    {


        // TODO: store the block information in a JSON or Lua File somewhere
        addBlock(0);  // Air
        addBlock(1); // Sample



    }


    public void addBlock(int blockIndex)
    {

        Block block = new Block(null, new Vector3Int(0,0,0), new Vector3Int(0, 0, 0),true, true);

        if (blockIndex == 0)
        {

            block.name = "Air";
            block.isSolid = false;
            block.isVisible = false;

        }

        if (blockIndex == 1)
        {
            block.name = "Sample";
            block.isSolid = true;
            block.isVisible = true;
        }

        blocks[blockIndex] = block;
    }
}
