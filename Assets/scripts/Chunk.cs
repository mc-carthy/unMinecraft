using System.Collections;
using UnityEngine;

public class Chunk {

    public Material cubeMat;
    public Block[,,] chunkData;
    public GameObject chunk;

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
    }

    private void BuildChunk()
    {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        // Create blocks
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if (Random.Range(0, 100) < 50)
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);                        
                    }
                    else
                    {
                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, chunk.gameObject, this);
                    }
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
