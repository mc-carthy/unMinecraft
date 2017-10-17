using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public Material cubeMat;

    private void Start()
    {
        StartCoroutine(BuildChunk(4, 4, 4));
    } 

    private IEnumerator BuildChunk(int sizeX, int sizeY, int sizeZ)
    {
        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    Block b = new Block(Block.BlockType.DIRT, pos, this.gameObject, cubeMat);
                    b.DrawCube();
                    yield return null;
                }   
            }   
        }
        CombineQuads();
    }

    private void CombineQuads()
    {
        // Combine meshes in children
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        // Create a new mesh on the parent
        MeshFilter newMeshFilter = (MeshFilter) gameObject.AddComponent(typeof(MeshFilter));
        newMeshFilter.mesh = new Mesh();

        // Add combined child meshes as parent's mesh
        newMeshFilter.mesh.CombineMeshes(combine);

        // Create a renderer for the parent
        MeshRenderer meshRenderer = (MeshRenderer) gameObject.AddComponent(typeof(MeshRenderer));
        meshRenderer.material = cubeMat;

        // Delete the children whose meshes we've combined
        foreach (Transform quad in transform)
        {
            Destroy(quad.gameObject);
        }
    }

}
