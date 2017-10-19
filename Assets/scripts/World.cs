using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public static int chunkSize = 8;
    public static int columnHeight = 4;
    public static int worldSize = 2;
    public static Dictionary<string, Chunk> chunks;
    
    public Material textureAtlas;

    private void Start()
    {
        chunks = new Dictionary<string, Chunk>();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        StartCoroutine(BuildWorld());
    }

    public static string BuildChunkName(Vector3 pos)
    {
        return ((int)pos.x + "-" + (int)pos.y + "-" + (int)pos.z);
    }

    private IEnumerator BuildChunkColumn()
    {
        for (int i = 0; i < columnHeight; i++)
        {
            Vector3 chunkPos = new Vector3(
                this.transform.position.x,
                i * chunkSize,
                this.transform.position.z
            );
            Chunk c = new Chunk(chunkPos, textureAtlas);
            c.chunk.transform.parent = transform;
            chunks.Add(c.chunk.name, c);        
        }

        foreach(KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }

    }

    private IEnumerator BuildWorld()
    {
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < columnHeight; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 chunkPos = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    Chunk c = new Chunk(chunkPos, textureAtlas);
                    c.chunk.transform.parent = transform;
                    chunks.Add(c.chunk.name, c);
                }
            }
        }

        foreach(KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }
    }

}
