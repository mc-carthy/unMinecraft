using Realtime.Messaging.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {

    public static int chunkSize = 16;
    public static int columnHeight = 16;
    public static int worldGenRadius = 7;
    public static uint maxCoroutines = 1000;
    public static ConcurrentDictionary<string, Chunk> chunks;
    public static List<string> toRemove = new List<string>();
    public Vector3 lastBuildPos;
    public GameObject player;
    public Material textureAtlas;

    private bool firstBuild = true;
    private CoroutineQueue queue;

    private void Start()
    {
        Vector3 playerPos = player.transform.position;
        player.transform.position = new Vector3(
            playerPos.x, 
            Utils.GenerateHeight(playerPos.x, playerPos.z) + 1, 
            playerPos.z
        );

        lastBuildPos = player.transform.position;
        player.SetActive(false);
        
        firstBuild = true;
        chunks = new ConcurrentDictionary<string, Chunk>();

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        queue = new CoroutineQueue(maxCoroutines, StartCoroutine);

        // Build first chunk
        BuildChunkAt(
            (int)player.transform.position.x / chunkSize,
            (int)player.transform.position.y / chunkSize,
            (int)player.transform.position.z / chunkSize
        );

        // Draw chunk
        queue.Run(DrawChunks());

        // Create wider world
        queue.Run(BuildRecursiveWorld(
            (int)player.transform.position.x / chunkSize,
            (int)player.transform.position.y / chunkSize,
            (int)player.transform.position.z / chunkSize,
            worldGenRadius
        ));
    }

    private void Update()
    {
        Vector3 movement = lastBuildPos - player.transform.position;
        if (movement.sqrMagnitude > (chunkSize * chunkSize))
        {
            lastBuildPos = player.transform.position;
            BuildNearPlayer();
        }

        if (!player.activeSelf)
        {
            player.SetActive(true);
            firstBuild = false;
        }

        queue.Run(DrawChunks());
        queue.Run(RemoveOldChunks());
    }

    public static string BuildChunkName(Vector3 pos)
    {
        return ((int)pos.x + "-" + (int)pos.y + "-" + (int)pos.z);
    }

    private void BuildChunkAt(int x, int y, int z)
    {
        Vector3 chunkPos = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);

        string n = BuildChunkName(chunkPos);
        Chunk c;

        if (!chunks.TryGetValue(n, out c))
        {
            c = new Chunk(chunkPos, textureAtlas);
            c.chunk.transform.parent = transform;
            chunks.TryAdd(c.chunk.name, c);
        }
    }

    private IEnumerator BuildRecursiveWorld(int x, int y, int z, int radius)
    {
        radius--;
        if (radius <= 0)
        {
            yield break;
        }

        // Build chunk front
        BuildChunkAt(x, y, z + 1);
        queue.Run(BuildRecursiveWorld(x, y, z + 1, radius));
        yield return null;

        // Build chunk back
        BuildChunkAt(x, y, z - 1);
        queue.Run(BuildRecursiveWorld(x, y, z - 1, radius));
        yield return null;

        // Build chunk right
        BuildChunkAt(x + 1, y, z);
        queue.Run(BuildRecursiveWorld(x + 1, y, z, radius));
        yield return null;

        // Build chunk left
        BuildChunkAt(x - 1, y, z);
        queue.Run(BuildRecursiveWorld(x - 1, y, z, radius));
        yield return null;

        // Build chunk up
        BuildChunkAt(x, y + 1, z);
        queue.Run(BuildRecursiveWorld(x, y + 1, z, radius));
        yield return null;

        // Build chunk down
        BuildChunkAt(x, y - 1, z);
        queue.Run(BuildRecursiveWorld(x, y - 1, z, radius));
        yield return null;
    }

    public void BuildNearPlayer()
    {
        StopCoroutine("BuildRecursiveWorld");
        queue.Run(BuildRecursiveWorld(
            (int)player.transform.position.x / chunkSize,
            (int)player.transform.position.y / chunkSize,
            (int)player.transform.position.z / chunkSize,
            worldGenRadius
        ));
    }

    private IEnumerator DrawChunks()
    {
        foreach(KeyValuePair<string, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
            }

            if (c.Value.chunk != null && (Vector3.Distance(player.transform.position, c.Value.chunk.transform.position) > (worldGenRadius * chunkSize)))
            {
                toRemove.Add(c.Key);
            }
            yield return null;
        }
    }

    private IEnumerator RemoveOldChunks()
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            string n = toRemove[i];
            Chunk c;
            if (chunks.TryGetValue(n, out c))
            {
                Destroy(c.chunk);
                chunks.TryRemove(n, out c);
                yield return null;
            }
        }
    }
}
