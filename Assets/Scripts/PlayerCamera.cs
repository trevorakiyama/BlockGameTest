using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {

        //transform.position = new Vector3(0, 60, -5);

        //Debug.LogFormat("Pos: {0}", this.transform.position);
        transform.position = new Vector3(0, 150, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;


        // move upwards at 1 unit per second


        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + delta * 5);


    }
}
