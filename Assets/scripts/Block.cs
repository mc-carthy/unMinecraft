using System;
using UnityEngine;

public class Block {

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
        STONE,
        DIAMOND,
        AIR
    };

    public bool isSolid;
    private BlockType blockType;
    private Vector3 pos;
    private Chunk owner;
    private GameObject parent;
    private Material cubeMat;

    private Vector2[,] blockUvs = {
        // 0 - Grass Top
        {
            new Vector2(0.125f, 0.375f), 
            new Vector2(0.1875f, 0.375f),
            new Vector2(0.125f, 0.4375f),
            new Vector2(0.1875f, 0.4375f)
        },
        // 1 - Grass Side
        {
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.25f, 0.9375f),
            new Vector2(0.1875f, 1.0f),
            new Vector2(0.25f, 1.0f)
        },
        // 2 - Dirt
        {
            new Vector2(0.125f, 0.9375f),
            new Vector2(0.1875f, 0.9375f),
            new Vector2(0.125f, 1.0f),
            new Vector2(0.1875f, 1.0f)
        },
        // 3 - Stone
        {
            new Vector2(0, 0.875f),
            new Vector2(0.0625f, 0.875f),
            new Vector2(0, 0.9375f),
            new Vector2(0.0625f, 0.9375f)
        },
        // 4 - Diamond
        {
            new Vector2(0.125f, 0.75f),
            new Vector2(0.1875f, 0.75f),
            new Vector2(0.125f, 0.8125f),
            new Vector2(0.1875f, 0.8125f)
        }
    };

    public Block(BlockType _blockType, Vector3 _pos, GameObject _parent, Chunk _owner)
    {
        blockType = _blockType;
        pos = _pos;
        parent = _parent;
        owner = _owner;
        isSolid = (_blockType == BlockType.AIR) ? false :true;
    }

    public void DrawCube()
    {
        if (blockType == BlockType.AIR)
        {
            return;
        }

        if (!HasSolidNeighbour((int)pos.x, (int)pos.y - 1, (int)pos.z))
        {
            CreateQuad(CubeSide.BOTTOM);
        }
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y + 1, (int)pos.z))
        {
            CreateQuad(CubeSide.TOP);
        }
        if (!HasSolidNeighbour((int)pos.x - 1, (int)pos.y, (int)pos.z))
        {
            CreateQuad(CubeSide.LEFT);
        }
        if (!HasSolidNeighbour((int)pos.x + 1, (int)pos.y, (int)pos.z))
        {
            CreateQuad(CubeSide.RIGHT);
        }
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z + 1))
        {
            CreateQuad(CubeSide.FRONT);
        }
        if (!HasSolidNeighbour((int)pos.x, (int)pos.y, (int)pos.z - 1))
        {
            CreateQuad(CubeSide.BACK);
        }
    }

    public bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunks;

        if (
            x < 0 ||  x >= World.chunkSize ||
            y < 0 ||  y >= World.chunkSize ||
            z < 0 ||  z >= World.chunkSize
        )
        // The block is in a neighbouring chunk
        {
            Vector3 neighbourChunkPos = parent.transform.position + 
                new Vector3(
                    (x - (int)pos.x) * World.chunkSize,
                    (y - (int)pos.y) * World.chunkSize,
                    (z - (int)pos.z) * World.chunkSize
                );

            string neighbourName = World.BuildChunkName(neighbourChunkPos);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            Chunk newChunk;
            if (World.chunks.TryGetValue(neighbourName, out newChunk))
            {
                chunks = newChunk.chunkData;
            }
            else
            {
                return false;
            }
        }
        // The block is in this chunk
        else
        {
            chunks = owner.chunkData;
        }

        try
        {
            return chunks[x, y, z].isSolid;
        }
        catch (System.IndexOutOfRangeException) {}

        return false;
    }

    private int ConvertBlockIndexToLocal(int i)
    {
        if (i == -1)
        {
            i = World.chunkSize - 1;
        }
        else if (i == World.chunkSize)
        {
            i = 0;
        }
        
        return i;
    }

    private void CreateQuad(CubeSide cubeSide)
    {
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh" + cubeSide.ToString();

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
        quad.transform.position = pos;
        quad.transform.parent = parent.transform;
        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;

        // This is not required unless we're showing individual quads instead of combining them
        // MeshRenderer meshRenderer = (MeshRenderer)quad.AddComponent(typeof(MeshRenderer));
        // meshRenderer.material = cubeMat;
    }

}
