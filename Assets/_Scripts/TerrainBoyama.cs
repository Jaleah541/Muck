using UnityEngine;

public class TerrainPainter : MonoBehaviour
{
    public Terrain terrain;
    public TerrainLayer grassLayer; // Inspector'dan ekle

    void Start()
    {
        if (terrain.terrainData.terrainLayers.Length == 0)
        {
            terrain.terrainData.terrainLayers = new TerrainLayer[] { grassLayer };
        }

        int mapSize = terrain.terrainData.alphamapResolution;
        float[,,] map = new float[mapSize, mapSize, 1]; // Sadece bir layer

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                map[x, y, 0] = 1; // %100 bu texture ile kapla
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, map);
    }
}
