﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {

    public static int chunkSize = 16;
    public static int columnHeight = 16;
    public static int worldGenRadius = 1;
    public static Dictionary<string, Chunk> chunks;
    
    public GameObject player;
    public Material textureAtlas;
    public Slider loadingSlider;
    public Camera uiCam;
    public Button playButton;

    private void Start()
    {
        player.SetActive(false);
        chunks = new Dictionary<string, Chunk>();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public void StartBuild()
    {
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
        int posX = (int)Mathf.Floor(player.transform.position.x / chunkSize);
        int posZ = (int)Mathf.Floor(player.transform.position.z / chunkSize);

        float totalChunks = (Mathf.Pow(worldGenRadius * 2 + 1, 2) * columnHeight) * 2;
        int processCount = 0;

        for (int z = -worldGenRadius; z <= worldGenRadius; z++)
        {
            for (int x = -worldGenRadius; x <= worldGenRadius; x++)
            {
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 chunkPos = new Vector3((x + posX) * chunkSize, y * chunkSize, (z + posZ) * chunkSize);
                    Chunk c = new Chunk(chunkPos, textureAtlas);
                    c.chunk.transform.parent = transform;
                    chunks.Add(c.chunk.name, c);
                    processCount++;
                    loadingSlider.value = (processCount / totalChunks) * 100;
                }
            }
        }

        foreach(KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            processCount++;
            loadingSlider.value = (processCount / totalChunks) * 100;
            yield return null;
        }
        player.SetActive(true);
        loadingSlider.gameObject.SetActive(false);
        uiCam.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
    }

}
