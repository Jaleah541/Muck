using UnityEngine;

public class HaritaOluşturucu : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float scale = 20;

    public float heightMultiplier = 5f;
    public int seed = 42;

    public AnimationCurve heightCurve;
    public bool BaşladıMı;

    void Start()
    {
        Mesh mesh = GenerateMyMesh(); // Artık mesh döndürüyor

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
            mf = gameObject.AddComponent<MeshFilter>();

        mf.mesh = mesh;

        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();

        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc == null)
            mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
    }

    Mesh GenerateMyMesh()  // <-- burası artık 'Mesh' döndürüyor
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        int[] triangles = new int[width * height * 6];

        for (int i = 0, z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = heightCurve.Evaluate(Mathf.PerlinNoise((x + seed) / scale, (z + seed) / scale)) * heightMultiplier;
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

        return mesh;
    }
}
