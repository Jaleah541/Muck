using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class HaritaOluşturucu : MonoBehaviour
{
    public Terrain terrain;                // Unity Terrain bileşeni atanmalı
    public int width = 1000;               // Genişlik (x ekseni)
    public int height = 1000;              // Uzunluk (z ekseni)
    public int resolution = 513;           // Terrain çözünürlüğü (2^n + 1)
    public float heightMultiplier = 10f;   // Yükseklik çarpanı
    public float noiseScale = 100f;        // Gürültü ölçeği
    public bool BaşladıMı=false;
    public GameObject LoadingCanvas;
    [SerializeField, HideInInspector]
    private int seed;

    void Start()
    {
        LoadingCanvas.SetActive(true);

        // Eğer Photon odasındaysak, oradaki seed’i al
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("seed"))
        {
            string seedText = PhotonNetwork.CurrentRoom.CustomProperties["seed"].ToString();
            int.TryParse(seedText, out seed);  // String'i int'e çevir
        }

        GenerateTerrain();
    }


    void GenerateTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(width, heightMultiplier, height);

        float[,] heights = new float[resolution, resolution];
        System.Random prng = new System.Random(seed);
        float offsetX = prng.Next(-100000, 100000);
        float offsetZ = prng.Next(-100000, 100000);

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float xCoord = (float)x / resolution * noiseScale + offsetX;
                float zCoord = (float)z / resolution * noiseScale + offsetZ;
                float noise = Mathf.PerlinNoise(xCoord, zCoord);
                heights[x, z] = noise;
            }
        }

        terrainData.SetHeights(0, 0, heights);
        BaşladıMı = true;
        LoadingCanvas.SetActive(false);
        Debug.Log("tamamlandı");
    }

}
