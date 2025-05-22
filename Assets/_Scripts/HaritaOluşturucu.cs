using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HaritaOluşturucu : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float scale = 20f;
    public float heightMultiplier = 5f;
    public int seed = 42;
    public AnimationCurve heightCurve;
    public bool BaşladıMı;
    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        int[] triangles = new int[width * height * 6];

        System.Random prng = new System.Random(seed);

        for (int i = 0, z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float sampleX = (x + prng.Next(-100000, 100000)) / scale;
                float sampleZ = (z + prng.Next(-100000, 100000)) / scale;

                float noise = Mathf.PerlinNoise(sampleX, sampleZ);
                float y = heightCurve.Evaluate(noise) * heightMultiplier;

                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        MeshFilter mf = GetComponent<MeshFilter>();
        MeshCollider mc = GetComponent<MeshCollider>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        mf.mesh = mesh;
        mc.sharedMesh = mesh;

        // Basit malzeme ayarlaması (istersen shader değiştirirsin)
        if (mr.sharedMaterial == null)
        {
            mr.material = new Material(Shader.Find("Standard"));
        }
    }
}
