using UnityEngine;
using Photon.Pun;

public class HaritaYapıcı : MonoBehaviourPunCallbacks
{
    public Terrain terrain;
    public GameObject LoadingMenu;
    public RoomPrefab roomPrefab;

    [Header("Seed Ayarları")]
    public string kullanilanSeed;

    [Header("Terrain Ayarları")]
    public float terrainSize = 200f;
    public float maxHeight = 40f;
    public float waterLevel = 0.3f;

    [Header("Doku Katmanları")]
    public TerrainLayer mudLayer;   // Toprak
    public TerrainLayer grassLayer; // Çim
    public TerrainLayer rockLayer;  // Kaya

    public bool BaşladıMı { get; private set; } = false;

    void Start()
    {
        LoadingMenu.SetActive(true);

        if (string.IsNullOrEmpty(kullanilanSeed))
        {
            if (roomPrefab != null && roomPrefab.Seedİsmi != null)
                kullanilanSeed = roomPrefab.Seedİsmi.text;
            else
                kullanilanSeed = System.DateTime.Now.Ticks.ToString();
        }

        Random.InitState(kullanilanSeed.GetHashCode());
        HaritaOluştur();

        BaşladıMı = true;
        LoadingMenu.SetActive(false);
    }

    void HaritaOluştur()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain eksik!");
            return;
        }

        int width = terrain.terrainData.heightmapResolution;
        int height = terrain.terrainData.heightmapResolution;
        float[,] heights = new float[width, height];

        int octaves = 5;
        float persistence = 0.5f;
        float lacunarity = 2f;
        float scale = 100f;

        AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float offsetX = Random.Range(0f, 9999f);
        float offsetY = Random.Range(0f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x / (float)width) * scale * frequency + offsetX;
                    float sampleY = (y / (float)height) * scale * frequency + offsetY;
                    float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    noiseHeight += perlin * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                float normalized = Mathf.InverseLerp(-1f, 1f, noiseHeight);
                heights[x, y] = heightCurve.Evaluate(normalized);
            }
        }

        // Falloff uygulaması
        float[,] falloff = GenerateFalloff(width);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = Mathf.Clamp01(heights[x, y] - falloff[x, y]);
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
        terrain.terrainData.size = new Vector3(terrainSize, maxHeight, terrainSize);

        AddWater(heights);
        ApplyTextures();
    }

    float[,] GenerateFalloff(int size)
    {
        float[,] map = new float[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float nx = i / (float)size * 2 - 1;
                float ny = j / (float)size * 2 - 1;
                float value = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
                map[i, j] = Mathf.Pow(value, 3); // Falloff eğrisi
            }
        }
        return map;
    }

    void AddWater(float[,] heights)
    {
        int res = terrain.terrainData.heightmapResolution;
        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                if (heights[x, y] < waterLevel)
                {
                    heights[x, y] = 0f;
                }
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);
    }

    void ApplyTextures()
    {
        var data = terrain.terrainData;
        data.terrainLayers = new TerrainLayer[] { mudLayer, grassLayer, rockLayer };

        int w = data.alphamapWidth;
        int h = data.alphamapHeight;
        float[,,] alphas = new float[w, h, 3];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                float normX = x / (float)w;
                float normY = y / (float)h;
                float height = data.GetInterpolatedHeight(normX, normY) / maxHeight;

                float[] mix = new float[3];
                mix[0] = Mathf.Clamp01(1 - (height / 0.3f));                   // Mud
                mix[1] = 1 - Mathf.Abs((height - 0.45f) / 0.15f);               // Grass
                mix[2] = Mathf.Clamp01((height - 0.6f) / 0.4f);                 // Rock

                float sum = mix[0] + mix[1] + mix[2];
                for (int i = 0; i < 3; i++)
                    alphas[x, y, i] = mix[i] / sum;
            }
        }

        data.SetAlphamaps(0, 0, alphas);
    }
}
