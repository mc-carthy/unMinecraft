﻿using System.Collections;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public GameObject block;
    public int worldSize;

    private void Start()
    {
        StartCoroutine(BuildWorld());
    }

    public IEnumerator BuildWorld()
    {
        for(int z = 0; z < worldSize; z++)
        {
            for(int y = 0; y < worldSize; y++)
            {
                for(int x = 0; x < worldSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    GameObject cube = Instantiate(block, pos, Quaternion.identity);
                    cube.name = (x + "_" + y + "_" + z);
                    cube.transform.parent = gameObject.transform;
                }
                yield return null;
            }
        }
    }

}
