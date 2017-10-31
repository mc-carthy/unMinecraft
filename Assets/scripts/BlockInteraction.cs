using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour {

    private Camera cam;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10))
            {
                Vector3 hitBlock = hit.point - (hit.normal / 2.0f);

                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                List<string> updates = new List<string>();
                float thisChunkX = hit.collider.gameObject.transform.position.x;
                float thisChunkY = hit.collider.gameObject.transform.position.y;
                float thisChunkZ = hit.collider.gameObject.transform.position.z;

                updates.Add(hit.collider.gameObject.name);

                if (x == 0) updates.Add(World.BuildChunkName(new Vector3(thisChunkX - World.chunkSize, thisChunkY, thisChunkZ)));
                if (x == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisChunkX + World.chunkSize, thisChunkY, thisChunkZ)));
                if (y == 0) updates.Add(World.BuildChunkName(new Vector3(thisChunkX, thisChunkY - World.chunkSize, thisChunkZ)));
                if (y == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisChunkX, thisChunkY + World.chunkSize, thisChunkZ)));
                if (z == 0) updates.Add(World.BuildChunkName(new Vector3(thisChunkX, thisChunkY, thisChunkZ - World.chunkSize)));
                if (z == World.chunkSize - 1) updates.Add(World.BuildChunkName(new Vector3(thisChunkX, thisChunkY, thisChunkZ + World.chunkSize)));


                foreach(string chunkName in updates)
                {
                    Chunk c;

                    if (World.chunks.TryGetValue(chunkName, out c))
                    {
                        DestroyImmediate(c.chunk.GetComponent<MeshFilter>());
                        DestroyImmediate(c.chunk.GetComponent<MeshRenderer>());
                        DestroyImmediate(c.chunk.GetComponent<Collider>());
                        c.chunkData[x, y, z].SetType(Block.BlockType.AIR);
                        c.DrawChunk();
                    }
                }
            }
        }
    }

}
