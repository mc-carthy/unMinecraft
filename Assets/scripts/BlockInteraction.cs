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

                Chunk c;

                if (World.chunks.TryGetValue(hit.collider.gameObject.name, out c))
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
