using System.Collections;
using UnityEngine;

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

}
