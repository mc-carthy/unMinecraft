using UnityEngine;

public class CreateQuads : MonoBehaviour {

    enum CubeSide {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    };

    public enum BlockType {
        GRASS,
        DIRT,
        STONE
    };

    public Material cubeMat;
    public BlockType blockType;

    private Vector2[,] blockUvs = {
        // Grass Top
        {
            new Vector2(0.125f, 0.375f), 
            new Vector2(0.1875f, 0.375f),
            new Vector2(0.125f, 0.4375f),
            new Vector2(0.1875f, 0.4375f)
        },
        // Grass Side
        {
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.25f, 0.9375f),
            new Vector2(0.1875f, 1.0f),
            new Vector2(0.25f, 1.0f)
        },
        // Dirt
        {
            new Vector2(0.125f, 0.9375f),
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.125f, 1.0f),
            new Vector2(0.1875f, 1.0f)
        },
        // Stone
        {
            new Vector2(0, 0.875f),
            new Vector2(0.0625f, 0.875f),
            new Vector2(0, 0.9375f),
            new Vector2(0.0625f, 0.9375f)
        }
    };

    private void Start()
    {
        CreateCube();
        CombineQuads();
    }

    private void CreateCube()
    {
        CreateQuad(CubeSide.BOTTOM);
        CreateQuad(CubeSide.TOP);
        CreateQuad(CubeSide.LEFT);
        CreateQuad(CubeSide.RIGHT);
        CreateQuad(CubeSide.FRONT);
        CreateQuad(CubeSide.BACK);
    }

    private void CreateQuad(CubeSide cubeSide)
    {
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh";

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];
        
        // All possible vertices of a cube
        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        // All possible UVs§
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (blockType == BlockType.GRASS && cubeSide == CubeSide.TOP)
        {
            uv00 = blockUvs[0, 0];
            uv10 = blockUvs[0, 1];
            uv01 = blockUvs[0, 2];
            uv11 = blockUvs[0, 3];
        }
        // The bottom side of a grass block is dirt
        else if (blockType == BlockType.GRASS && cubeSide == CubeSide.TOP)
        {
            uv00 = blockUvs[(int)(BlockType.DIRT + 1), 0];
            uv10 = blockUvs[(int)(BlockType.DIRT + 1), 1];
            uv01 = blockUvs[(int)(BlockType.DIRT + 1), 2];
            uv11 = blockUvs[(int)(BlockType.DIRT + 1), 3];
        }
        else
        {
            uv00 = blockUvs[(int)(blockType + 1), 0];
            uv10 = blockUvs[(int)(blockType + 1), 1];
            uv01 = blockUvs[(int)(blockType + 1), 2];
            uv11 = blockUvs[(int)(blockType + 1), 3];
        }

        switch(cubeSide)
        {
            case CubeSide.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1};
                break;
            case CubeSide.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1};
                break;
            case CubeSide.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1};
                break;
            case CubeSide.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1};
                break;
            case CubeSide.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1};
                break;
            case CubeSide.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1};
                break;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.parent = this.gameObject.transform;
        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = (MeshRenderer)quad.AddComponent(typeof(MeshRenderer));
        meshRenderer.material = cubeMat;
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
