using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public static int chunkSize = 16;
    public static int columnHeight = 16;
    public static Dictionary<string, Chunk> chunks;
    
    public Material textureAtlas;

    private void Start()
    {
        chunks = new Dictionary<string, Chunk>();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        StartCoroutine(BuildChunkColumn());
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

}
