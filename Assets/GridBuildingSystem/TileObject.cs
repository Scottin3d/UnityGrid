using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : GenericPoolableObject
{
    float startTime;
    float timeout = 5f;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > timeout)
        {
            // Returning object
            //ReturnToPool();
        }
    }
}
