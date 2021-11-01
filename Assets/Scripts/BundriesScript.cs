using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundriesScript : MonoBehaviour
{
    private int maxX;
    private int minX;
    private int maxY;
    private int minY;
    private int maxZ;
    private int minZ;

    // Start is called before the first frame update
    void Start()
    {
        maxX = 555;
        minX = 55;
        maxY = 35;
        minY = 0;
        maxZ = 475;
        minZ = -15;
    }

    // Update is called once per frame
    void Update()
    {

        // X POS
        if (transform.position.x > maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }

        if (transform.position.x < minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }

        // Y POS
        if (transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }

        if (transform.position.y < minY)
        {
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        }

        // Z POS
        if (transform.position.z > maxZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }

        if (transform.position.z < minZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
        }
    }
}