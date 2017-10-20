using UnityEngine;

public class Utils {
    // This should be less than (World.chunkSize * World.columnHeight)
    static int maxHeight = 30;
    static float smooth = 0.01f;
    static int octaves = 4;
    static float persistence = 0.5f;

    public static int GenerateStoneHeight(float x, float z)
    {
        return (int)Map(0, maxHeight - 5, 0, 1, fBM(x * smooth * 2, z * smooth * 2, octaves + 1, persistence));
    }

    public static int GenerateHeight(float x, float z)
    {
        return (int)Map(0, maxHeight, 0, 1, fBM(x * smooth, z * smooth, octaves, persistence));
    }

    private static float fBM(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }

        return (total / maxValue);
    }

    public static float fBM3D(float x, float y, float z)
    {
        float XY = fBM(x * smooth * 10, y * smooth * 10, 3, 0.5f);
        float YZ = fBM(y * smooth * 10, z * smooth * 10, 3, 0.5f);
        float XZ = fBM(x * smooth * 10, z * smooth * 10, 3, 0.5f);
        float YX = fBM(y * smooth * 10, x * smooth * 10, 3, 0.5f);
        float ZY = fBM(z * smooth * 10, y * smooth * 10, 3, 0.5f);
        float ZX = fBM(z * smooth * 10, x * smooth * 10, 3, 0.5f);

        return ((XY + YZ + XZ + YX + ZY + ZX) / 6.0f);
    }

    private static float Map(float min, float max, float omin, float omax, float value)
    {
        return Mathf.Lerp(min, max, Mathf.InverseLerp(omin, omax, value));
    }
}