using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using UnityEngine;

[Serializable]
class BlockData {
    
    public Block.BlockType [,,] matrix;

    
    public BlockData() {}

    public BlockData(Block[,,] b)
    {
        matrix = new Block.BlockType[World.chunkSize, World.chunkSize, World.chunkSize];

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    matrix[x, y, z] = b[x, y, z].blockType;
                }   
            }   
        }
    }
}


public class Chunk {

    public enum ChunkStatus {
        DRAW,
        DONE,
        KEEP
    }

    public Material cubeMat;
    public Block[,,] chunkData;
    public GameObject chunk;
    public ChunkStatus status;
    public float touchedTime;

    private BlockData blockData;

    public Chunk() {}

    public Chunk(Vector3 pos, Material _cubeMat)
    {
        chunk = new GameObject(World.BuildChunkName(pos));
        chunk.transform.position = pos;
        cubeMat = _cubeMat;
        BuildChunk();
    }

    public void DrawChunk()
    {
        // Draw blocks
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunkData[x, y, z].DrawCube();
                }   
            }   
        }
        CombineQuads();
        MeshCollider meshCollider = (MeshCollider)chunk.gameObject.AddComponent(typeof(MeshCollider));
        meshCollider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
        status = ChunkStatus.DONE;
    }

    private void BuildChunk()
    {
        bool dataFromFile = false;
        dataFromFile = Load();

        touchedTime = Time.time;
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        // Create blocks
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + chunk.transform.position.x);
                    int worldY = (int)(y + chunk.transform.position.y);
                    int worldZ = (int)(z + chunk.transform.position.z);

                    if (dataFromFile)
                    {
                        chunkData[x, y, z] = new Block(blockData.matrix[x, y, z], pos, chunk.gameObject, this);
                        continue;
                    }

                    int surfaceHeight = Utils.GenerateHeight(worldX, worldZ);
                    
                    if (worldY == 0)
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.BEDROCK, pos, chunk.gameObject, this);
                    }
                    else if (Utils.fBM3D(worldX, worldY, worldZ, 0.1f, 3) < 0.42f)
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);
                    }
                    else if (worldY <= Utils.GenerateStoneHeight(worldX, worldZ))
                    {
                        if (Utils.fBM3D(worldX, worldY, worldZ, 0.01f, 2) < 0.35f && worldY < 30)
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, chunk.gameObject, this);                        
                        }
                        else if (Utils.fBM3D(worldX, worldY, worldZ, 0.03f, 3) < 0.41f && worldY < 20)
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.REDSTONE, pos, chunk.gameObject, this);                        
                        }
                        else
                        {
                            chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, chunk.gameObject, this);                        
                        }
                    }
                    else if (worldY == surfaceHeight)
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, chunk.gameObject, this);                        
                    }
                    else if (worldY < surfaceHeight)
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, chunk.gameObject, this);                        
                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);
                    }

                    status = ChunkStatus.DRAW;
                }   
            }   
        }
    }

    private void CombineQuads()
    {
        // Combine meshes in children
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        // Create a new mesh on the parent
        MeshFilter newMeshFilter = (MeshFilter) chunk.gameObject.AddComponent(typeof(MeshFilter));
        newMeshFilter.mesh = new Mesh();

        // Add combined child meshes as parent's mesh
        newMeshFilter.mesh.CombineMeshes(combine);

        // Create a renderer for the parent
        MeshRenderer meshRenderer = (MeshRenderer) chunk.gameObject.AddComponent(typeof(MeshRenderer));
        meshRenderer.material = cubeMat;

        // Delete the children whose meshes we've combined
        foreach (Transform quad in chunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }

    private string BuildChunkFileName(Vector3 v)
    {
        return Application.persistentDataPath + "/saveData/Chunk_" +
            (int)v.x + "_" +
            (int)v.y + "_" +
            (int)v.z + "_" +
            World.chunkSize + "_" +
            World.worldGenRadius + "_" +
            ".dat";
    }

    private bool Load()
    {
        string chunkFile = BuildChunkFileName(chunk.transform.position);
        if (File.Exists(chunkFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.Open);
            blockData = new BlockData();
            blockData = (BlockData) bf.Deserialize(file);
            file.Close();
            return true;
        }
        return false;
    }

    public void Save()
    {
        string chunkFile = BuildChunkFileName(chunk.transform.position);

        if (!File.Exists(chunkFile))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
        blockData = new BlockData(chunkData);
        bf.Serialize(file, blockData);
        file.Close();
    }

}
