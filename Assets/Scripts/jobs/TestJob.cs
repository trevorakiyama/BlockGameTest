using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;


public struct TestJob : IJob
{

    public NativeList<int> temp;

    public void Execute()
    {



        temp.Resize(10, NativeArrayOptions.UninitializedMemory);
        temp[9] = 99;
    }
}
